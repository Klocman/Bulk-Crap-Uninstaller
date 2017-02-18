Public Class U3SettingsProvider
    Inherits PortableSettingsProvider

    Private Const U3APPDATAPATH As String = "U3_APP_DATA_PATH"

    Overrides Function GetAppSettingsPath() As String
        'Get the environment variable set by the U3 application for a pointer to its data
        Try
            Return Environment.GetEnvironmentVariable(U3APPDATAPATH)
        Catch ex As Exception
            'Not running in a U3 environment, just return the application path
            Return MyBase.GetAppSettingsPath
        End Try
    End Function
End Class
