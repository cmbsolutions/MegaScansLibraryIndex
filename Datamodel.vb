Namespace Models
    Public Class BareGroup
        Public Property assets As List(Of BareMinimum)
    End Class
    Public Class BareMinimum
        Public Property name As String
        Public Property tags As List(Of String)
        Public Property id As String
        Public Property pack As Pack
    End Class

    Public Class Pack
        Public Property id As String
        Public Property name As String
    End Class

    Public Class ExtendedDatatable
        Public Property dt As DataTable
        Public Property TotalRecords As Integer
        Public Property CurrentPage As Integer
        Public Property TotalPages As Integer
    End Class

    Public Class DataFilter
        Public Property filterfieldname As String
        Public Property filterComparison As String
        Public Property filterCompareTo As New List(Of String)
    End Class
End Namespace


