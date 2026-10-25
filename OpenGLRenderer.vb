Imports System.ComponentModel
Imports OpenTK.Mathematics
Imports OpenTK
Imports OpenTK.GLControl
Imports OpenTK.Graphics.OpenGL

''' <summary>
''' OpenGL-based 3D renderer that renders FBX scene data on a UserControl.
''' Uses OpenTK (OpenGL bindings for .NET) to draw meshes with basic Phong lighting.
''' Supports camera orbit, zoom, and pan controls via mouse interaction.
''' 
''' Required NuGet packages:
''' - OpenTK
''' - OpenTK.Windowing.Desktop
''' - OpenTK.GLWGL
''' </summary>
''' <summary>
''' Rendering view modes for the 3D scene.
''' </summary>
Public Enum EViewMode
    Textured
    Wireframe
    Vertex
End Enum

Public Class OpenGLRenderer
    Inherits UserControl

    ''' <summary>
    ''' Camera orbit angles (in radians).
    ''' </summary>
    Private _theta As Single = 0.5F      ' Horizontal angle
    Private _phi As Single = Math.PI / 6.0F ' Vertical angle
    Private _radius As Single = 10.0F       ' Distance to target

    ''' <summary>
    ''' Camera properties
    ''' </summary>
    Private _cameraTarget As New OpenTK.Mathematics.Vector3(0, 0, 0)
    Private _lastMouseX As Single = 0F
    Private _lastMouseY As Single = 0F

    ''' <summary>
    ''' Mouse state for interaction
    ''' </summary>
    Private _isOrbiting As Boolean = False
    Private _isPanning As Boolean = False

    ''' <summary>
    ''' VBO and IBO handles
    ''' </summary>
    Private _vboHandles As New List(Of Integer)()
    Private _iboHandles As New List(Of Integer)()
    Private _vaoHandles As New List(Of Integer)()
    Private _shaderProgram As Integer = 0
    Private _isInitialized As Boolean = False

    ''' <summary>
    ''' Texture state. _textureHandles tracks every GL texture we created for disposal.
    ''' _textureMap caches a resolved file path -> GL texture handle.
    ''' </summary>
    Private _textureHandles As New List(Of Integer)()
    Private _textureMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' Directory the FBX was loaded from, used to resolve relative texture paths.
    ''' </summary>
    Private _baseDirectory As String = ""

    ''' <summary>
    ''' Lighting settings
    ''' </summary>
    Private _lightDirection As OpenTK.Mathematics.Vector3 = New OpenTK.Mathematics.Vector3(1, 1, -1).Normalized()
    Private _ambientIntensity As Single = 0.2F
    Private _diffuseIntensity As Single = 0.8F
    Private _viewMode As EViewMode = EViewMode.Textured

    Public _sd As Assimp.Scene
    Private _gl As GLControl

    Public Sub New()
        If LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            Return
        End If
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If DesignMode OrElse LicenseManager.UsageMode = LicenseUsageMode.Designtime Then
            Return
        End If

        If _gl IsNot Nothing Then Return

        _gl = New GLControl() With {
            .Dock = DockStyle.Fill
        }

        Controls.Add(_gl)

        AddHandler _gl.Paint, AddressOf GL_Paint
        AddHandler _gl.Resize, AddressOf GL_Resize
        AddHandler _gl.MouseMove, AddressOf OnMouseMove
        AddHandler _gl.MouseDown, AddressOf OnMouseDown
        AddHandler _gl.MouseUp, AddressOf OnMouseUp
        AddHandler _gl.MouseWheel, AddressOf OnMouseWheel
    End Sub

    ''' <summary>
    ''' Sets the directory used to resolve relative texture paths (usually the FBX file's folder).
    ''' </summary>
    Public Sub SetTextureBaseDirectory(directory As String)
        If directory IsNot Nothing Then
            _baseDirectory = directory
        End If
    End Sub

    Public Sub LoadFbxScene(scene As Assimp.Scene)
        _sd = scene
        If _gl IsNot Nothing AndAlso _gl.IsHandleCreated Then
            _gl.MakeCurrent()
            If _textureHandles.Count > 0 Then
                ClearTextures()
            End If
            InitializeOpenGL()
            RebuildMeshBuffers()
            _gl.Invalidate()
        End If
    End Sub

    Private Sub GL_Resize(sender As Object, e As EventArgs)
        If Not _gl.IsHandleCreated Then Return

        _gl.MakeCurrent()

        OpenTK.Graphics.OpenGL4.GL.Viewport(0, 0,
                    _gl.ClientSize.Width,
                    _gl.ClientSize.Height)

        _gl.Invalidate()
    End Sub

    Private Sub GL_Paint(sender As Object, e As PaintEventArgs)
        _gl.MakeCurrent()

        OpenTK.Graphics.OpenGL4.GL.Clear(OpenTK.Graphics.OpenGL4.ClearBufferMask.ColorBufferBit Or
                 OpenTK.Graphics.OpenGL4.ClearBufferMask.DepthBufferBit)

        RenderFrame()

        _gl.SwapBuffers()
    End Sub

    Private Sub InitializeOpenGL()
        ' Set up OpenGL state
        _gl.MakeCurrent()
        OpenTK.Graphics.OpenGL4.GL.ClearColor(0.15F, 0.2F, 0.3F, 1.0F)
        OpenTK.Graphics.OpenGL4.GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.DepthTest)
        OpenTK.Graphics.OpenGL4.GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.CullFace)
        OpenTK.Graphics.OpenGL4.GL.CullFace(OpenTK.Graphics.OpenGL4.TriangleFace.Back)
        OpenTK.Graphics.OpenGL4.GL.FrontFace(OpenTK.Graphics.OpenGL4.FrontFaceDirection.Ccw)

        ' Create the shader program
        _shaderProgram = CreateShaderProgram()

        ' Generate VBOs/IBOs for scene meshes
        RebuildMeshBuffers()
    End Sub

    Private Function CreateShaderProgram() As Integer

        Dim vertexSource As String = "
#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec2 aTexCoord;

uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

out vec3 vNormal;
out vec3 vWorldPos;
out vec2 vTexCoord;

void main()
{
    vec4 worldPos = uModelMatrix * vec4(aPosition, 1.0);

    vWorldPos = worldPos.xyz;
    vNormal = mat3(uModelMatrix) * aNormal;
    vTexCoord = aTexCoord;

    gl_Position =
        uProjectionMatrix *
        uViewMatrix *
        worldPos;
}"
        Dim fragmentSource As String = "
#version 330 core

in vec3 vNormal;
in vec3 vWorldPos;
in vec2 vTexCoord;

uniform vec3 uLightDir;
uniform vec3 uAmbientColor;
uniform vec3 uDiffuseColor;
uniform vec3 uSpecularColor;
uniform float uShininess;
uniform vec3 uCameraPos;
uniform sampler2D uDiffuseMap;
uniform int uHasTexture;

out vec4 fragColor;

void main()
{
    vec3 normal = normalize(vNormal);
    vec3 lightDir = normalize(-uLightDir);

    // Albedo: use the texture when present, otherwise the flat diffuse color
    vec3 albedo;
    if (uHasTexture > 0)
    {
        albedo = texture(uDiffuseMap, vTexCoord).rgb;
    }
    else
    {
        albedo = uDiffuseColor;
    }

    // Ambient lighting
    vec3 ambient = uAmbientColor * albedo;

    // Diffuse lighting
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = albedo * diff;

    // Specular lighting
    vec3 viewDir = normalize(uCameraPos - vWorldPos);
    vec3 reflectDir = reflect(-lightDir, normal);

    float spec = pow(
        max(dot(viewDir, reflectDir), 0.0),
        max(uShininess, 1.0)
    );

    vec3 specular = uSpecularColor * spec;

    fragColor = vec4(ambient + diffuse + specular, 1.0);
}"

        Dim vertShader = CompileShader(OpenTK.Graphics.OpenGL4.ShaderType.VertexShader, vertexSource)

        Dim fragShader = CompileShader(OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader, fragmentSource)

        Dim program = OpenTK.Graphics.OpenGL4.GL.CreateProgram()

        OpenTK.Graphics.OpenGL4.GL.AttachShader(program, vertShader)
        OpenTK.Graphics.OpenGL4.GL.AttachShader(program, fragShader)
        OpenTK.Graphics.OpenGL4.GL.LinkProgram(program)

        Dim status As Integer
        OpenTK.Graphics.OpenGL4.GL.GetProgram(program, OpenTK.Graphics.OpenGL4.GetProgramParameterName.LinkStatus, status)

        Dim log = OpenTK.Graphics.OpenGL4.GL.GetProgramInfoLog(program)

        OpenTK.Graphics.OpenGL4.GL.DeleteShader(vertShader)
        OpenTK.Graphics.OpenGL4.GL.DeleteShader(fragShader)

        If status = 0 Then
            OpenTK.Graphics.OpenGL4.GL.DeleteProgram(program)
            Throw New Exception($"Shader linking failed: {log}")
        End If

        Return program
    End Function

    Private Function CompileShader(shaderType As OpenTK.Graphics.OpenGL4.ShaderType, source As String) As Integer

        Dim shader = OpenTK.Graphics.OpenGL4.GL.CreateShader(shaderType)

        OpenTK.Graphics.OpenGL4.GL.ShaderSource(shader, source)
        OpenTK.Graphics.OpenGL4.GL.CompileShader(shader)

        Dim status As Integer
        OpenTK.Graphics.OpenGL4.GL.GetShader(shader, OpenTK.Graphics.OpenGL4.ShaderParameter.CompileStatus, status)

        If status = 0 Then
            Dim log = OpenTK.Graphics.OpenGL4.GL.GetShaderInfoLog(shader)
            OpenTK.Graphics.OpenGL4.GL.DeleteShader(shader)

            Throw New Exception($"Shader compilation failed: {log}")
        End If

        Return shader
    End Function

    Private Sub RebuildMeshBuffers()
        _gl.MakeCurrent()
        ' Delete existing buffers
        For Each vbo In _vboHandles
            OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(vbo)
        Next
        _vboHandles.Clear()

        For Each ibo In _iboHandles
            OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(ibo)
        Next
        _iboHandles.Clear()

        For Each vao In _vaoHandles
            OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(vao)
        Next
        _vaoHandles.Clear()

        If _sd Is Nothing Then Return

        ' Create buffers for each mesh
        For i As Integer = 0 To _sd.MeshCount - 1
            Dim mesh As Assimp.Mesh = _sd.Meshes.Item(i)

            If mesh.VertexCount = 0 Then Continue For

            ' Interleave vertex data: position (3 floats) + normal (3 floats) + texCoord (2 floats)
            Dim vertices As Single() = New Single((mesh.VertexCount * 8) - 1) {}

            For j As Integer = 0 To mesh.VertexCount - 1
                Dim vert As Assimp.Vector3D = mesh.Vertices(j)
                Dim idx As Integer = j * 8

                ' Position
                vertices(idx + 0) = vert.X
                vertices(idx + 1) = vert.Y
                vertices(idx + 2) = vert.Z

                ' Normal
                If j < mesh.Normals.Count Then
                    Dim normal As Assimp.Vector3D = mesh.Normals(j)
                    vertices(idx + 3) = normal.X
                    vertices(idx + 4) = normal.Y
                    vertices(idx + 5) = normal.Z
                Else
                    vertices(idx + 3) = 0F
                    vertices(idx + 4) = 1.0F
                    vertices(idx + 5) = 0F
                End If

                ' UV coordinates
                If mesh.TextureCoordinateChannelCount > 0 Then
                    Dim uv As Assimp.Vector3D = mesh.TextureCoordinateChannels(0)(j)
                    vertices(idx + 6) = uv.X
                    vertices(idx + 7) = uv.Y
                Else
                    vertices(idx + 6) = 0F
                    vertices(idx + 7) = 0F
                End If
            Next

            Debug.WriteLine($"Mesh: {mesh.Name}")
            Debug.WriteLine($"VertexCount: {mesh.VertexCount}")
            Debug.WriteLine($"Array length: {vertices.Length}")
            Debug.WriteLine($"Buffer bytes: {CLng(vertices.Length) * 4L}")

            _gl.MakeCurrent()
            Dim vbo As Integer = OpenTK.Graphics.OpenGL4.GL.GenBuffer()
            OpenTK.Graphics.OpenGL4.GL.BindBuffer(OpenTK.Graphics.OpenGL4.BufferTarget.ArrayBuffer, vbo)

            Dim bufferSize As Integer = vertices.Length * 4
            _gl.MakeCurrent()
            OpenTK.Graphics.OpenGL4.GL.BufferData(OpenTK.Graphics.OpenGL4.BufferTarget.ArrayBuffer,
              CType(bufferSize, IntPtr),
              vertices,
              OpenTK.Graphics.OpenGL4.BufferUsageHint.StaticDraw)

            _vboHandles.Add(vbo)

            ' Create index buffer (EBO) if indices exist
            If mesh.GetIndices().Length > 0 Then
                _gl.MakeCurrent()
                Dim ibo As Integer = OpenTK.Graphics.OpenGL4.GL.GenBuffer()
                OpenTK.Graphics.OpenGL4.GL.BindBuffer(OpenTK.Graphics.OpenGL4.BufferTarget.ArrayBuffer, ibo)

                Dim indexArray = mesh.GetIndices()
                Dim ibobufferSize As Integer = indexArray.Length * 4
                _gl.MakeCurrent()
                OpenTK.Graphics.OpenGL4.GL.BufferData(OpenTK.Graphics.OpenGL4.BufferTarget.ArrayBuffer,
                  CType(ibobufferSize, IntPtr),
                  indexArray,
                  OpenTK.Graphics.OpenGL4.BufferUsageHint.StaticDraw)

                _iboHandles.Add(ibo)
            End If
        Next

        _gl.MakeCurrent()

        For i As Integer = 0 To _vboHandles.Count - 1

            Dim vao As Integer = OpenTK.Graphics.OpenGL4.GL.GenVertexArray()
            OpenTK.Graphics.OpenGL4.GL.BindVertexArray(vao)

            OpenTK.Graphics.OpenGL4.GL.BindBuffer(OpenTK.Graphics.OpenGL4.BufferTarget.ArrayBuffer, _vboHandles(i))

            ' Position
            OpenTK.Graphics.OpenGL4.GL.EnableVertexAttribArray(0)
            OpenTK.Graphics.OpenGL4.GL.VertexAttribPointer(0, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, False, 32, IntPtr.Zero)

            ' Normal
            OpenTK.Graphics.OpenGL4.GL.EnableVertexAttribArray(1)
            OpenTK.Graphics.OpenGL4.GL.VertexAttribPointer(1, 3, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, False, 32, New IntPtr(12))

            ' UV
            OpenTK.Graphics.OpenGL4.GL.EnableVertexAttribArray(2)
            OpenTK.Graphics.OpenGL4.GL.VertexAttribPointer(2, 2, OpenTK.Graphics.OpenGL4.VertexAttribPointerType.Float, False, 32, New IntPtr(24))

            ' The EBO binding is stored in the VAO
            If i < _iboHandles.Count AndAlso _iboHandles(i) <> 0 Then
                OpenTK.Graphics.OpenGL4.GL.BindBuffer(OpenTK.Graphics.OpenGL4.BufferTarget.ElementArrayBuffer, _iboHandles(i))
            End If

            _vaoHandles.Add(vao)

        Next
    End Sub

    ''' <summary>
    ''' Resolves the texture file referenced by a material (slot 0 = diffuse) to an absolute path.
    ''' Returns Nothing if the material has no texture.
    ''' </summary>
    Private Function ResolveTexturePath(mat As Assimp.Material) As String
        Try
            ' Iterate the material's texture slots and return the first base-color texture we can resolve.
            For Each slot In mat.GetAllMaterialTextures()
                Dim texType As Assimp.TextureType = slot.TextureType
                ' The diffuse/base-color map is what we sample in the shader; ignore other slot types.
                If texType <> Assimp.TextureType.Diffuse AndAlso
                   texType <> Assimp.TextureType.Ambient Then
                    Continue For
                End If

                Dim filename As String = slot.FilePath
                If String.IsNullOrWhiteSpace(filename) Then
                    Continue For
                End If

                Dim path As String = filename
                If Not System.IO.Path.IsPathRooted(path) AndAlso _baseDirectory <> "" Then
                    path = System.IO.Path.Combine(_baseDirectory, filename)
                End If
                If System.IO.File.Exists(path) Then
                    Return path
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("ResolveTexturePath failed: " & ex.Message)
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Returns the GL texture handle for the given file, creating and caching it on first use.
    ''' Returns 0 if the file cannot be loaded.
    ''' </summary>
    Private Function GetOrCreateTexture(path As String) As Integer
        Dim key As String = path.ToLowerInvariant()
        If _textureMap.ContainsKey(key) Then
            Return _textureMap(key)
        End If

        Dim texHandle As Integer = 0
        Try
            Using bmp As New Bitmap(path)
                _gl.MakeCurrent()
                texHandle = OpenTK.Graphics.OpenGL4.GL.GenTexture()
                OpenTK.Graphics.OpenGL4.GL.BindTexture(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, texHandle)

                OpenTK.Graphics.OpenGL4.GL.TexParameterI(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D,
                    OpenTK.Graphics.OpenGL4.TextureParameterName.TextureWrapS, OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat)
                OpenTK.Graphics.OpenGL4.GL.TexParameterI(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D,
                    OpenTK.Graphics.OpenGL4.TextureParameterName.TextureWrapT, OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat)
                OpenTK.Graphics.OpenGL4.GL.TexParameterI(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D,
                    OpenTK.Graphics.OpenGL4.TextureParameterName.TextureMinFilter, OpenTK.Graphics.OpenGL4.TextureMinFilter.LinearMipmapLinear)
                OpenTK.Graphics.OpenGL4.GL.TexParameterI(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D,
                    OpenTK.Graphics.OpenGL4.TextureParameterName.TextureMagFilter, OpenTK.Graphics.OpenGL4.TextureMagFilter.Linear)

                ' Bitmaps are BGRA; upload with the matching pixel format to avoid byte swapping.
                Dim data As System.Drawing.Imaging.BitmapData = bmp.LockBits(New System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                                                            System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                                            System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                Try
                    OpenTK.Graphics.OpenGL4.GL.TexImage2D(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, 0,
                        OpenTK.Graphics.OpenGL4.PixelInternalFormat.Rgba8,
                        bmp.Width, bmp.Height, 0,
                        OpenTK.Graphics.OpenGL4.PixelFormat.Bgra,
                        OpenTK.Graphics.OpenGL4.PixelType.UnsignedByte,
                        data.Scan0)
                Finally
                    bmp.UnlockBits(data)
                End Try

                OpenTK.Graphics.OpenGL4.GL.GenerateMipmap(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D)
            End Using

            OpenTK.Graphics.OpenGL4.GL.BindTexture(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, 0)

            _textureMap(key) = texHandle
            _textureHandles.Add(texHandle)
        Catch ex As Exception
            Debug.WriteLine("GetOrCreateTexture failed for " & path & ": " & ex.Message)
            If texHandle <> 0 AndAlso _textureHandles.Contains(texHandle) Then
                OpenTK.Graphics.OpenGL4.GL.DeleteTexture(texHandle)
                _textureHandles.Remove(texHandle)
            End If
            texHandle = 0
        End Try

        Return texHandle
    End Function

    ''' <summary>
    ''' Deletes every GL texture we created (used when a new scene replaces the current one).
    ''' </summary>
    Private Sub ClearTextures()
        For Each t In _textureHandles
            OpenTK.Graphics.OpenGL4.GL.DeleteTexture(t)
        Next
        _textureHandles.Clear()
        _textureMap.Clear()
    End Sub

    Private Sub RenderFrame()


        _gl.MakeCurrent()
        ' Clear the screen
        OpenTK.Graphics.OpenGL4.GL.Clear(OpenTK.Graphics.OpenGL4.ClearBufferMask.ColorBufferBit Or OpenTK.Graphics.OpenGL4.ClearBufferMask.DepthBufferBit)

        If _sd Is Nothing Then Return
        If _vboHandles.Count = 0 Then Return

        _gl.MakeCurrent()
        ' Activate shader program
        OpenTK.Graphics.OpenGL4.GL.UseProgram(_shaderProgram)

        ' Configure rendering mode based on view mode
        Select Case _viewMode
            Case EViewMode.Wireframe
                OpenTK.Graphics.OpenGL4.GL.PolygonMode(OpenTK.Graphics.OpenGL4.TriangleFace.FrontAndBack, OpenTK.Graphics.OpenGL4.PolygonMode.Line)
                OpenTK.Graphics.OpenGL4.GL.Disable(OpenTK.Graphics.OpenGL4.EnableCap.CullFace)
            Case EViewMode.Vertex
                OpenTK.Graphics.OpenGL4.GL.PolygonMode(OpenTK.Graphics.OpenGL4.TriangleFace.FrontAndBack, OpenTK.Graphics.OpenGL4.PolygonMode.Point)
                OpenTK.Graphics.OpenGL4.GL.Disable(OpenTK.Graphics.OpenGL4.EnableCap.CullFace)
                OpenTK.Graphics.OpenGL4.GL.PointSize(3.0F)
            Case Else
                OpenTK.Graphics.OpenGL4.GL.PolygonMode(OpenTK.Graphics.OpenGL4.TriangleFace.FrontAndBack, OpenTK.Graphics.OpenGL4.PolygonMode.Fill)
                OpenTK.Graphics.OpenGL4.GL.Enable(OpenTK.Graphics.OpenGL4.EnableCap.CullFace)
        End Select

        Dim linked As Integer
        GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, linked)

        Dim currentProgram As Integer
        GL.GetInteger(GetPName.CurrentProgram, currentProgram)

        ' Get scene bounding box for centering and sizing
        Dim bounds As BoundingBoxF = ComputeSceneBounds()
        Dim sceneCenter As Vector3 = bounds.Center
        Dim sceneSize As Single = Math.Max(bounds.Size.X, Math.Max(bounds.Size.Y, bounds.Size.Z))
        If sceneSize < 0.01F Then sceneSize = 1.0F

        ' Update camera to look at the center of the scene
        If _cameraTarget = Vector3.Zero Then _cameraTarget = sceneCenter
        ComputeOrbitCamera(sceneCenter, sceneSize)

        ' Set up view and projection matrices
        Dim aspect As Single = CSng(_gl.ClientSize.Width / Math.Max(1, _gl.ClientSize.Height))
        ' Update shader uniforms
        Dim viewMatrix As Matrix4 = Matrix4.LookAt(_cameraPosition, _cameraTarget, OpenTK.Mathematics.Vector3.UnitY)

        Dim projectionMatrix As Matrix4 = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4, aspect, 0.01F, sceneSize * 100.0F)



        ' Helper to get uniform locations
        Dim projLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uProjectionMatrix")
        Dim viewLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uViewMatrix")
        Dim lightDirLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uLightDir")
        Dim ambientColorLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uAmbientColor")
        Dim cameraPosLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uCameraPos")
        Dim modelLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uModelMatrix")

        Dim identity As OpenTK.Mathematics.Matrix4 = OpenTK.Mathematics.Matrix4.Identity

        OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(modelLoc, False, identity)


        OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(projLoc, False, projectionMatrix)
        OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(viewLoc, False, viewMatrix)

        OpenTK.Graphics.OpenGL4.GL.Uniform3(lightDirLoc, _lightDirection.X, _lightDirection.Y, _lightDirection.Z)
        OpenTK.Graphics.OpenGL4.GL.Uniform3(ambientColorLoc, _ambientIntensity, _ambientIntensity, _ambientIntensity)
        OpenTK.Graphics.OpenGL4.GL.Uniform3(cameraPosLoc, _cameraPosition.X, _cameraPosition.Y, _cameraPosition.Z)

        ' Render each mesh
        For i As Integer = 0 To Math.Min(_vboHandles.Count - 1, _sd.MeshCount - 1)
            Dim mesh As Assimp.Mesh = _sd.Meshes.Item(i)
            ' Set material properties (default gray if no material) using managed OpenTK API
            Dim diffuseColorLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uDiffuseColor")
            Dim specularColorLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uSpecularColor")
            Dim shininessLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uShininess")

            ' Bind the material's diffuse texture (if any) to texture unit 0.
            Dim texHandle As Integer = 0
            Dim hasTexture As Boolean = False
            If i < _sd.Materials.Count Then
                Dim mat As Assimp.Material = _sd.Materials(mesh.MaterialIndex)
                If diffuseColorLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform3(diffuseColorLoc, mat.ColorDiffuse.R, mat.ColorDiffuse.G, mat.ColorDiffuse.B)
                End If
                If specularColorLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform3(specularColorLoc, mat.ColorSpecular.R, mat.ColorSpecular.G, mat.ColorSpecular.B)
                End If
                If shininessLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform1(shininessLoc, CSng(mat.Shininess))
                End If

                Dim texPath As String = ResolveTexturePath(mat)
                If texPath IsNot Nothing Then
                    texHandle = GetOrCreateTexture(texPath)
                    hasTexture = (texHandle <> 0)
                End If
            Else
                If diffuseColorLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform3(diffuseColorLoc, 0.6F, 0.6F, 0.7F)
                End If
                If specularColorLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform3(specularColorLoc, 0.5F, 0.5F, 0.5F)
                End If
                If shininessLoc >= 0 Then
                    OpenTK.Graphics.OpenGL4.GL.Uniform1(shininessLoc, 32.0F)
                End If
            End If

            ' Configure texture state for this mesh
            OpenTK.Graphics.OpenGL4.GL.ActiveTexture(OpenTK.Graphics.OpenGL4.TextureUnit.Texture0)
            If hasTexture Then
                OpenTK.Graphics.OpenGL4.GL.BindTexture(OpenTK.Graphics.OpenGL4.TextureTarget.Texture2D, texHandle)
            End If
            Dim hasTextureLoc As Integer = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(_shaderProgram, "uHasTexture")
            If hasTextureLoc >= 0 Then
                OpenTK.Graphics.OpenGL4.GL.Uniform1(hasTextureLoc, If(hasTexture, 1, 0))
            End If

            ' Bind the VAO for this mesh
            OpenTK.Graphics.OpenGL4.GL.BindVertexArray(_vaoHandles(i))

            If i < _iboHandles.Count AndAlso _iboHandles(i) <> 0 Then
                Dim activeVAO As Integer
                GL.GetInteger(GetPName.VertexArrayBinding, activeVAO)

                Dim activeEBO As Integer
                GL.GetInteger(GetPName.ElementArrayBufferBinding, activeEBO)

                OpenTK.Graphics.OpenGL4.GL.DrawElements(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, mesh.GetIndices().Length, OpenTK.Graphics.OpenGL4.DrawElementsType.UnsignedInt, IntPtr.Zero)
            Else
                OpenTK.Graphics.OpenGL4.GL.DrawArrays(OpenTK.Graphics.OpenGL4.PrimitiveType.Triangles, 0, mesh.VertexCount)
            End If

            ' Unbind VAO
            OpenTK.Graphics.OpenGL4.GL.BindVertexArray(0)
        Next
    End Sub

    Private ReadOnly Property _cameraPosition As OpenTK.Mathematics.Vector3
        Get
            Return New OpenTK.Mathematics.Vector3(
                _cameraTarget.X + _radius * Math.Sin(_phi) * Math.Cos(_theta),
                _cameraTarget.Y + _radius * Math.Cos(_phi),
                _cameraTarget.Z + _radius * Math.Sin(_phi) * Math.Sin(_theta))
        End Get
    End Property

    Private Sub ComputeOrbitCamera(center As OpenTK.Mathematics.Vector3, size As Single)
        ' Set default orbit distance based on scene size
        If _radius < 1 Then
            _radius = Math.Max(size * 2, 5)
        End If
    End Sub

    Private Function ComputeSceneBounds() As BoundingBoxF
        Dim bb As New BoundingBoxF()

        If _sd Is Nothing OrElse _sd.MeshCount = 0 Then
            Return bb
        End If

        For Each mesh In _sd.Meshes
            For Each v As OpenTK.Mathematics.Vector3 In mesh.Vertices.Select(Function(x) New OpenTK.Mathematics.Vector3(x.X, x.Y, x.Z))
                bb.ExpandBy(v)
            Next
        Next

        Return bb
    End Function

#Region "Event Handlers"

    Private Shadows Sub OnMouseMove(sender As Object, e As MouseEventArgs)
        If _isOrbiting Then
            ' Orbit camera based on mouse movement
            Dim deltaX As Single = (e.X - _lastMouseX) * 0.01F
            Dim deltaY As Single = (e.Y - _lastMouseY) * 0.01F

            _theta -= deltaX
            _phi -= deltaY
            _phi = Math.Max(0.01F, Math.Min(Math.PI - 0.01F, _phi)) ' Clamp vertical angle

            _gl.Invalidate() ' Force redraw
        ElseIf _isPanning Then
            ' Pan camera target based on mouse movement
            Dim deltaX As Single = (e.X - _lastMouseX) * 0.01F * _radius / 100
            Dim deltaY As Single = (e.Y - _lastMouseY) * 0.01F * _radius / 100

            ' Calculate pan offsets based on camera orientation
            Dim forward As OpenTK.Mathematics.Vector3 = (_cameraTarget - _cameraPosition).Normalized()
            Dim right As OpenTK.Mathematics.Vector3 = OpenTK.Mathematics.Vector3.Cross(forward, OpenTK.Mathematics.Vector3.UnitY).Normalized()
            Dim up As OpenTK.Mathematics.Vector3 = OpenTK.Mathematics.Vector3.Cross(right, forward).Normalized()

            _cameraTarget -= right * deltaX + up * deltaY

            _gl.Invalidate() ' Force redraw
        End If

        _lastMouseX = CSng(e.X)
        _lastMouseY = CSng(e.Y)
    End Sub

    Private Shadows Sub OnMouseDown(sender As Object, e As MouseEventArgs)
        Select Case e.Button
            Case MouseButtons.Left
                _isOrbiting = True
                _lastMouseX = CSng(e.X)
                _lastMouseY = CSng(e.Y)
            Case MouseButtons.Right
                _isPanning = True
                _lastMouseX = CSng(e.X)
                _lastMouseY = CSng(e.Y)
        End Select
    End Sub

    Private Shadows Sub OnMouseUp(sender As Object, e As MouseEventArgs)
        Select Case e.Button
            Case MouseButtons.Left
                _isOrbiting = False
            Case MouseButtons.Right
                _isPanning = False
        End Select
    End Sub

    Private Shadows Sub OnMouseWheel(sender As Object, e As MouseEventArgs)
        ' Zoom in/out by adjusting radius
        Dim zoomFactor As Single = 1.0F - CSng(e.Delta) * 0.001F
        _radius *= zoomFactor
        _radius = Math.Max(0.5F, Math.Min(_radius, 1000.0F)) ' Clamp zoom range

        _gl.Invalidate() ' Force redraw
    End Sub

    Private Shadows Sub OnResize(sender As Object, e As EventArgs)
        If IsHandleCreated Then OpenTK.Graphics.OpenGL4.GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height)
        _gl.Invalidate()
    End Sub

#End Region

#Region "Public API"

    Public Sub SetViewMode(mode As EViewMode)
        _viewMode = mode
        If _gl IsNot Nothing Then _gl.Invalidate() ' Force redraw
    End Sub
    ''' <summary>
    ''' Called when a new FBX scene is loaded or changed. Rebuilds all OpenGL buffers.
    ''' </summary>
    Public Sub RefreshScene(newScene As Assimp.Scene)
        If _textureHandles.Count > 0 Then
            ClearTextures()
        End If

        If _vboHandles.Count > 0 Then
            For Each vbo In _vboHandles
                OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(vbo)
            Next
            _vboHandles.Clear()
        End If

        If _iboHandles.Count > 0 Then
            For Each ibo In _iboHandles
                OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(ibo)
            Next
            _iboHandles.Clear()
        End If

        If _vaoHandles.Count > 0 Then
            For Each vao In _vaoHandles
                OpenTK.Graphics.OpenGL4.GL.DeleteBuffer(vao)
            Next
            _vaoHandles.Clear()
        End If

        _sd = newScene

        If IsHandleCreated Then
            RebuildMeshBuffers()
            _gl.Invalidate() ' Force immediate redraw
        End If
    End Sub

    ''' <summary>
    ''' Resets the camera to a default position.
    ''' </summary>
    Public Sub ResetCamera()
        _cameraTarget = New OpenTK.Mathematics.Vector3(0, 5, 10)
        _theta = 0.5F
        _phi = Math.PI / 6
        _radius = 10.0F

        _gl.Invalidate() ' Force redraw
    End Sub

    ''' <summary>
    ''' Sets the lighting direction and intensities.
    ''' </summary>
    Public Sub SetLighting(direction As Vector3, ambientIntensity As Single, diffuseIntensity As Single)
        _lightDirection = direction.Normalized()
        _ambientIntensity = ambientIntensity
        _diffuseIntensity = diffuseIntensity

        _gl.Invalidate() ' Force redraw
    End Sub

#End Region

End Class

''' <summary>
''' An axis-aligned bounding box in single-precision world units.
''' Used to compute a scene's center and overall size for camera framing.
''' </summary>
Public Class BoundingBoxF
    Private _minX As Single = Single.MaxValue
    Private _minY As Single = Single.MaxValue
    Private _minZ As Single = Single.MaxValue
    Private _maxX As Single = Single.MinValue
    Private _maxY As Single = Single.MinValue
    Private _maxZ As Single = Single.MinValue
    Private _isEmpty As Boolean = True

    Public Sub New()
    End Sub

    Public ReadOnly Property Center As OpenTK.Mathematics.Vector3
        Get
            If _isEmpty Then Return New OpenTK.Mathematics.Vector3()
            Return New OpenTK.Mathematics.Vector3(
                (_minX + _maxX) / 2.0F,
                (_minY + _maxY) / 2.0F,
                (_minZ + _maxZ) / 2.0F)
        End Get
    End Property

    Public ReadOnly Property Size As OpenTK.Mathematics.Vector3
        Get
            If _isEmpty Then Return New OpenTK.Mathematics.Vector3()
            Return New OpenTK.Mathematics.Vector3(
                _maxX - _minX,
                _maxY - _minY,
                _maxZ - _minZ)
        End Get
    End Property

    Public Sub ExpandBy(p As OpenTK.Mathematics.Vector3)
        _minX = Math.Min(_minX, p.X)
        _minY = Math.Min(_minY, p.Y)
        _minZ = Math.Min(_minZ, p.Z)
        _maxX = Math.Max(_maxX, p.X)
        _maxY = Math.Max(_maxY, p.Y)
        _maxZ = Math.Max(_maxZ, p.Z)
        _isEmpty = False
    End Sub
End Class

''' <summary>
''' Extension method for normalizing a Vector3.
''' </summary>
Public Module MathExtensions
    <System.Runtime.CompilerServices.Extension>
    Public Function Normalized(vec As OpenTK.Mathematics.Vector3) As OpenTK.Mathematics.Vector3
        Dim length As Single = Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z)
        If length < 0.001 Then Return New OpenTK.Mathematics.Vector3(0, 1, 0) ' Default up vector
        Return New OpenTK.Mathematics.Vector3(vec.X / length, vec.Y / length, vec.Z / length)
    End Function
End Module

