Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports MegascansLibraryIndex.Models
Imports Microsoft.Data.Sqlite
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports SQLitePCL

Public Class DataContext
    Private CurrentConnection As SqliteConnection
    Private TagsDict As New Dictionary(Of Integer, String)
    Private IndexDict As New Dictionary(Of String, String)

    Sub New()
        Batteries.Init()
        CurrentConnection = New SqliteConnection("Data Source=megascanslibraryindex.db;")
    End Sub

    Public Sub BuildFileIndex()
        Dim base As String = "d:\quixelAssets\Original"
        Dim dlerror As String = "d:\quixelAssets\Original\missing.txt"
        Dim outputfile As String = $"{base}\missing_"
        Dim fileCount As Integer = 0
        Dim missing As New List(Of String)

        For Each line In IO.File.ReadAllLines(dlerror)
            missing.Add(line)
        Next

        Dim newJson As New JObject()
        Dim assetArray As New JArray

        For Each jsonfile In IO.Directory.GetFiles(base, "*.json", SearchOption.TopDirectoryOnly).ToList
            Dim jsonData As JObject = JObject.Parse(IO.File.ReadAllText(jsonfile))
            Dim assets As JArray = CType(jsonData("assets"), JArray)

            For Each asset As JObject In assets
                If asset("id") IsNot Nothing AndAlso missing.Contains(asset("id").ToString()) Then
                    ' Create a new JSON object with the matched asset
                    assetArray.Add(asset)
                End If

                If assetArray.Count >= 200 Then
                    newJson("assets") = assetArray
                    IO.File.WriteAllText($"{outputfile}{fileCount * 200 + 1}-{fileCount * 200 + assetArray.Count}.json", newJson.ToString)
                    fileCount += 1
                    assetArray = New JArray
                End If
            Next
        Next

        If assetArray.Count > 0 Then
            newJson("assets") = assetArray
            IO.File.WriteAllText($"{outputfile}{fileCount * 200 + 1}-{fileCount * 200 + assetArray.Count}.json", newJson.ToString)
        End If
    End Sub
    Public Async Sub StartIndex()
        Try
            Dim basefolder As String = "H:\megascanslibrary\CustomDownloads"
            Await CurrentConnection.OpenAsync()

            Using transaction = CurrentConnection.BeginTransaction
                Dim insertmegascancommand = CurrentConnection.CreateCommand
                insertmegascancommand.CommandText = "INSERT OR IGNORE INTO megascans (id,description,previewimage,pack_id,resolution,category,dgvlocation,totalsize,filecount) VALUES (@id,@description,@previewimage,@pack_id,@resolution,@category,@dgvlocation,@totalsize,@filecount)"

                Dim param1 = insertmegascancommand.CreateParameter
                param1.ParameterName = "@id"
                insertmegascancommand.Parameters.Add(param1)

                Dim param2 = insertmegascancommand.CreateParameter
                param2.ParameterName = "@description"
                insertmegascancommand.Parameters.Add(param2)

                Dim param3 = insertmegascancommand.CreateParameter
                param3.ParameterName = "@previewimage"
                insertmegascancommand.Parameters.Add(param3)

                Dim param6 = insertmegascancommand.CreateParameter
                param6.ParameterName = "@pack_id"
                insertmegascancommand.Parameters.Add(param6)

                Dim param7 = insertmegascancommand.CreateParameter
                param7.ParameterName = "@resolution"
                insertmegascancommand.Parameters.Add(param7)

                Dim param8 = insertmegascancommand.CreateParameter
                param8.ParameterName = "@category"
                insertmegascancommand.Parameters.Add(param8)

                Dim param9 = insertmegascancommand.CreateParameter
                param9.ParameterName = "@dgvlocation"
                insertmegascancommand.Parameters.Add(param9)

                Dim param10 = insertmegascancommand.CreateParameter
                param10.ParameterName = "@totalsize"
                insertmegascancommand.Parameters.Add(param10)

                Dim param11 = insertmegascancommand.CreateParameter
                param11.ParameterName = "@filecount"
                insertmegascancommand.Parameters.Add(param11)

                Dim command2 = CurrentConnection.CreateCommand
                command2.CommandText = "SELECT last_insert_rowid()"

                Dim insertmegascantags = CurrentConnection.CreateCommand
                insertmegascantags.CommandText = "INSERT OR IGNORE INTO megascantags (megascan_id,tag_id) VALUES (@megascan_id,@tag_id)"

                Dim param4 = insertmegascancommand.CreateParameter
                param4.ParameterName = "@megascan_id"
                insertmegascantags.Parameters.Add(param4)

                Dim param5 = insertmegascancommand.CreateParameter
                param5.ParameterName = "@tag_id"
                insertmegascantags.Parameters.Add(param5)

                Dim RegexObj As New Regex("([^\\]+)\\([^\\]+)\\", RegexOptions.IgnoreCase)

                For Each jsonfile In IO.Directory.EnumerateFiles(basefolder, "*.json", New IO.EnumerationOptions With {
                                                                 .RecurseSubdirectories = True,
                                                                 .MaxRecursionDepth = 10
                                                                 })

                    transaction.Save("Insert-record")

                    Dim jsonData = JsonConvert.DeserializeObject(Of Models.BareMinimum)(IO.File.ReadAllText(jsonfile))
                    Dim previewImage = IO.Directory.GetFiles(IO.Path.GetDirectoryName(jsonfile), "*preview.png", IO.SearchOption.TopDirectoryOnly).ToList
                    If jsonData Is Nothing Then Continue For
                    If jsonData.pack IsNot Nothing Then
                        param6.Value = InsertPack(jsonData.pack)
                    Else
                        param6.Value = DBNull.Value
                    End If
                    param1.Value = jsonData.id
                    param2.Value = jsonData.name
                    If previewImage IsNot Nothing AndAlso previewImage.Count > 0 Then
                        param3.Value = previewImage.First.Remove(0, basefolder.Length + 1)
                    Else
                        param3.Value = DBNull.Value
                    End If

                    param9.Value = IO.Path.GetDirectoryName(jsonfile)
                    param10.Value = (From d In IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(jsonfile), "*.*", SearchOption.AllDirectories) Select New IO.FileInfo(d)).Sum(Function(c) c.Length)
                    param11.Value = (From d In IO.Directory.EnumerateFiles(IO.Path.GetDirectoryName(jsonfile), "*.*", SearchOption.AllDirectories)).Count

                    param7.Value = RegexObj.Match(jsonfile.Remove(0, basefolder.Length + 1)).Groups(1).Value
                    param8.Value = RegexObj.Match(jsonfile.Remove(0, basefolder.Length + 1)).Groups(2).Value

                    insertmegascancommand.ExecuteNonQuery()

                    Dim megascan_id = CInt(command2.ExecuteScalar())

                    InsertTags(jsonData.tags)

                    For Each t In jsonData.tags
                        If t.Trim.Length = 0 Then Continue For
                        param4.Value = megascan_id
                        param5.Value = TagsDict.First(Function(c) c.Value = t.Trim).Key
                        insertmegascantags.ExecuteNonQuery()
                    Next


                    transaction.Release("Insert-record")

                Next
                transaction.Commit()
            End Using
            CurrentConnection.Close()
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub InsertTags(tags As List(Of String))

        Dim command = CurrentConnection.CreateCommand
        command.CommandText = "INSERT OR IGNORE INTO tags (tag) VALUES (@tag)"

        Dim command2 = CurrentConnection.CreateCommand
        command2.CommandText = "SELECT last_insert_rowid()"

        Dim param = command.CreateParameter
        param.ParameterName = "@tag"
        command.Parameters.Add(param)

        For Each t In tags
            If t.Trim.Length = 0 Then Continue For

            param.Value = t.Trim
            command.ExecuteNonQuery()

            If Not TagsDict.ContainsValue(t.Trim) Then
                TagsDict.Add(CInt(command2.ExecuteScalar()), t.Trim)
            End If
        Next
    End Sub


    Private Function InsertPack(pack As Pack) As Integer

        Dim command = CurrentConnection.CreateCommand
        command.CommandText = "INSERT OR IGNORE INTO packs (name) VALUES (@name)"

        Dim command2 = CurrentConnection.CreateCommand
        command2.CommandText = "SELECT last_insert_rowid()"

        Dim param = command.CreateParameter
        param.ParameterName = "@name"
        command.Parameters.Add(param)

        Dim param2 = command2.CreateParameter
        param2.ParameterName = "@name"
        command2.Parameters.Add(param2)


        param.Value = pack.name
        Dim rows = command.ExecuteNonQuery()

        If rows > 0 Then
            Return CInt(command2.ExecuteScalar())
        Else
            command2.CommandText = "SELECT pack_id FROM packs WHERE name=@name"
            param2.Value = pack.name
            Return CInt(command2.ExecuteScalar())
        End If
    End Function

    Public Function GetTags() As Dictionary(Of Integer, String)
        Dim newDict As New Dictionary(Of Integer, String)

        CurrentConnection.Open()

        Using command As New SqliteCommand("SELECT tag_id, tag FROM tags ORDER BY tag;", CurrentConnection)
            Using result = command.ExecuteReader
                While result.Read
                    newDict.Add(result.GetInt32(0), result.GetString(1))
                End While
            End Using
        End Using

        Return newDict
    End Function

    Public Function GetPacks() As Dictionary(Of Integer, String)
        Dim newDict As New Dictionary(Of Integer, String)

        CurrentConnection.Open()

        Using command As New SqliteCommand("SELECT pack_id, name FROM packs ORDER BY name;", CurrentConnection)
            Using result = command.ExecuteReader
                While result.Read
                    newDict.Add(result.GetInt32(0), result.GetString(1))
                End While
            End Using
        End Using

        Return newDict
    End Function

    Public Function GetFilteredByTags(filter As List(Of String), resos As List(Of String), Optional Page As Integer = 0) As ExtendedDatatable
        CurrentConnection.Open()
        Dim edt As New ExtendedDatatable With {
            .dt = GetDataTable()
        }
        Dim filters = "'" & String.Join("','", filter) & "'"
        Dim resoFilters = $" AND resolution IN ('{String.Join("','", resos)}') AND category IN ('{String.Join("','", resos)}')"

        Dim sql = $"SELECT m.megascan_id, m.id, m.description, m.previewimage, p.name AS packname, ifnull((SELECT tpm.assettags FROM tags_per_megascan AS tpm WHERE tpm.megascan_id=m.megascan_id), '') AS assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount FROM megascans AS m LEFT JOIN packs AS p ON (p.pack_id = m.pack_id) JOIN megascantags AS mst ON (mst.megascan_id = m.megascan_id) JOIN tags AS t ON (mst.tag_id = t.tag_id) WHERE t.tag IN ({filters}) {resoFilters}"
        edt.TotalRecords = GetTotalCount(sql)
        edt.TotalPages = Math.Ceiling(edt.TotalRecords / 100)
        edt.CurrentPage = Math.Min(Math.Max(0, Page), edt.TotalPages)
        sql = $"SELECT * FROM ({sql}) LIMIT 100 OFFSET {Page * 100}"

        Using command As New SqliteCommand(sql, CurrentConnection)
            edt.dt.Load(command.ExecuteReader)
        End Using

        Return edt
    End Function

    Public Function GetFilteredByPacks(packfilter As List(Of String), resos As List(Of String), Optional Page As Integer = 0) As ExtendedDatatable
        CurrentConnection.Open()
        Dim edt As New ExtendedDatatable With {
            .dt = GetDataTable()
        }
        Dim packfilters = "'" & String.Join("','", packfilter) & "'"
        Dim resoFilters = $" AND resolution IN ('{String.Join("','", resos)}') AND category IN ('{String.Join("','", resos)}')"

        Dim sql = $"Select m.megascan_id, m.id, m.description, m.previewimage, p.name As packname, ifnull((Select tpm.assettags FROM tags_per_megascan As tpm WHERE tpm.megascan_id=m.megascan_id), '') As assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount  FROM megascans As m JOIN packs As p On (p.pack_id = m.pack_id) WHERE p.name In ({packfilters}){resoFilters}"
        edt.TotalRecords = GetTotalCount(sql)
        edt.TotalPages = Math.Ceiling(edt.TotalRecords / 100)
        edt.CurrentPage = Math.Min(Math.Max(0, Page), edt.TotalPages)
        sql = $"SELECT * FROM ({sql}) LIMIT 100 OFFSET {Page * 100}"

        Using command As New SqliteCommand(sql, CurrentConnection)
            edt.dt.Load(command.ExecuteReader)
        End Using

        Return edt
    End Function

    Public Function GetFilteredByTagsAndPacks(tagfilter As List(Of String), packfilter As List(Of String), resos As List(Of String), Optional Page As Integer = 0) As ExtendedDatatable
        CurrentConnection.Open()
        Dim edt As New ExtendedDatatable With {
            .dt = GetDataTable()
        }
        Dim tagfilters = "'" & String.Join("','", tagfilter) & "'"
        Dim packfilters = "'" & String.Join("','", packfilter) & "'"
        Dim resoFilters = $" AND resolution IN ('{String.Join("','", resos)}') AND category IN ('{String.Join("','", resos)}')"

        Dim sw As New StringBuilder
        sw.Append($"SELECT m.megascan_id, m.id, m.description, m.previewimage, p.name AS packname, ifnull((SELECT tpm.assettags FROM tags_per_megascan AS tpm WHERE tpm.megascan_id=m.megascan_id), '') AS assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount  FROM megascans AS m LEFT JOIN packs AS p ON (p.pack_id = m.pack_id) JOIN megascantags AS mst ON (mst.megascan_id = m.megascan_id) JOIN tags AS t ON (mst.tag_id = t.tag_id) WHERE t.tag IN ({tagfilters}){resoFilters}")
        sw.Append(" UNION ")
        sw.Append($"Select m.megascan_id, m.id, m.description, m.previewimage, p.name As packname, ifnull((Select tpm.assettags FROM tags_per_megascan As tpm WHERE tpm.megascan_id=m.megascan_id), '') As assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount  FROM megascans As m JOIN packs As p On (p.pack_id = m.pack_id) WHERE p.name In ({packfilters}){resoFilters}")

        Dim sql = sw.ToString
        edt.TotalRecords = GetTotalCount(sql)
        edt.TotalPages = Math.Ceiling(edt.TotalRecords / 100)
        edt.CurrentPage = Math.Min(Math.Max(0, Page), edt.TotalPages)
        sql = $"SELECT * FROM ({sql}) LIMIT 100 OFFSET {Page * 100}"

        Try
            Using command As New SqliteCommand(sql, CurrentConnection)
                edt.dt.Load(command.ExecuteReader)
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        Return edt
    End Function

    Public Function GetFiltered(filters As List(Of DataFilter), Optional Page As Integer = 0) As ExtendedDatatable
        CurrentConnection.Open()
        Dim edt As New ExtendedDatatable With {
            .dt = GetDataTable()
        }

        Dim fb As New StringBuilder

        For Each dtf In filters
            If fb.Length > 0 Then fb.Append(" AND ")
            Select Case dtf.filterComparison
                Case "IN"
                    fb.Append($"{dtf.filterfieldname} IN ('{String.Join("','", dtf.filterCompareTo)}')")
                Case "LIKE"
                    fb.Append("(")
                    For Each fl In dtf.filterCompareTo
                        fb.Append($"{dtf.filterfieldname} LIKE '%{fl}%' OR ")
                    Next

                    fb.Remove(fb.Length - 4, 4)
                    fb.Append(")")
                Case "EQUALS"
                    fb.Append($"{dtf.filterfieldname} = '{dtf.filterCompareTo}'")
                Case "SOUNDEX"
                    Dim soundex = DbHelpers.SoundEx(dtf.filterCompareTo.First)
                    fb.Append($"{dtf.filterfieldname} = '{soundex}'")
                Case "BEGINSWITH"
                    fb.Append($"{dtf.filterfieldname} LIKE '{dtf.filterCompareTo.First}%'")
                Case "ENDSWITH"
                    fb.Append($"{dtf.filterfieldname} LIKE '%{dtf.filterCompareTo.First}'")
                Case "CONTAINS"
                    fb.Append($"{dtf.filterfieldname} LIKE '%{dtf.filterCompareTo.First}%'")
                Case Else

            End Select
        Next

        Dim sw As New StringBuilder
        sw.Append($"SELECT m.megascan_id, m.id, m.description, m.previewimage, ifnull(p.name, '') AS packname, ifnull(tpm.assettags, '') AS assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount FROM megascans AS m LEFT JOIN packs AS p ON (p.pack_id = m.pack_id) LEFT JOIN tags_per_megascan AS tpm ON (tpm.megascan_id = m.megascan_id) WHERE ")
        sw.Append(fb.ToString)

        Dim sql = sw.ToString
        edt.TotalRecords = GetTotalCount(sql)
        edt.TotalPages = Math.Ceiling(edt.TotalRecords / 100)
        edt.CurrentPage = Math.Min(Math.Max(0, Page), edt.TotalPages)
        sql = $"SELECT * FROM ({sql}) LIMIT 100 OFFSET {Page * 100}"

        Try
            Using command As New SqliteCommand(sql, CurrentConnection)
                edt.dt.Load(command.ExecuteReader)
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        Return edt
    End Function

    Public Function GetFilteredByNothing(Optional page As Integer = 0) As ExtendedDatatable
        CurrentConnection.Open()

        Dim edt As New ExtendedDatatable With {
            .dt = GetDataTable()
        }
        Try
            Dim sql = $"SELECT m.megascan_id, m.id, m.description, m.previewimage, ifnull(p.name, '') AS packname, ifnull(tpm.assettags, '') AS assettags, m.resolution, m.category, m.location, m.totalsize, m.filecount FROM megascans AS m LEFT JOIN packs AS p ON (p.pack_id = m.pack_id) LEFT JOIN tags_per_megascan AS tpm ON (tpm.megascan_id = m.megascan_id)"
            edt.TotalRecords = GetTotalCount(sql)
            edt.TotalPages = Math.Ceiling(edt.TotalRecords / 100)
            edt.CurrentPage = Math.Min(Math.Max(0, page), edt.TotalPages)
            sql = $"SELECT * FROM ({sql}) LIMIT 100 OFFSET {page * 100}"

            Using Command As New SqliteCommand(sql, CurrentConnection)
                edt.dt.Load(Command.ExecuteReader)
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try

        Return edt
    End Function

    Private Function GetTotalCount(sql As String) As Integer
        Dim results As Integer = 0
        Using command As New SqliteCommand($"SELECT COUNT(*) FROM ({sql});", CurrentConnection)
            results = Convert.ToInt32(command.ExecuteScalar())
        End Using

        Return results
    End Function

    Private Function GetDataTable() As DataTable
        Dim dt As New DataTable

        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "megascan_id",
                       .DataType = GetType(Integer),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "id",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "description",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "previewimage",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "packname",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "assettags",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "resolution",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "category",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "dgvlocation",
                       .DataType = GetType(String),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "totalsize",
                       .DataType = GetType(Long),
                       .Unique = False
                       })
        dt.Columns.Add(New DataColumn With {
                       .AllowDBNull = True,
                       .ColumnName = "filecount",
                       .DataType = GetType(Integer),
                       .Unique = False
                       })
        Return dt

    End Function

End Class
