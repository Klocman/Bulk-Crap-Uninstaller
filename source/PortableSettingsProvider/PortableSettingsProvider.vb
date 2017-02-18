Imports System.Collections.Specialized
Imports System.Configuration
Imports System.IO
Imports System.Xml


Public Class PortableSettingsProvider
    Inherits SettingsProvider

    Const SETTINGSROOT As String = "Settings" 'XML Root Node

    Public Overrides Sub Initialize(name As String, col As NameValueCollection)
        MyBase.Initialize(Me.ApplicationName, col)
    End Sub

    Public Overrides Property ApplicationName As String
        Get
            If Application.ProductName.Trim.Length > 0 Then
                Return Application.ProductName
            Else
                Dim fi As New FileInfo(Application.ExecutablePath)
                Return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length)
            End If
        End Get
        Set
            'Do nothing
        End Set
    End Property

    Overridable Function GetAppSettingsPath() As String
        'Used to determine where to store the settings
        Dim fi As New FileInfo(Application.ExecutablePath)
        Return fi.DirectoryName
    End Function

    Overridable Function GetAppSettingsFilename() As String
        'Used to determine the filename to store the settings
        Return ApplicationName & ".settings"
    End Function

    Public Overrides Sub SetPropertyValues(context As SettingsContext, propvals As SettingsPropertyValueCollection)
        'Iterate through the settings to be stored
        'Only dirty settings are included in propvals, and only ones relevant to this provider
        For Each propval As SettingsPropertyValue In propvals
            SetValue(propval)
        Next

        Try
            SettingsXML.Save(Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))
        Catch ex As Exception
            'Ignore if cant save, device been ejected
        End Try
    End Sub

    Public Overrides Function GetPropertyValues(context As SettingsContext, props As SettingsPropertyCollection) _
        As SettingsPropertyValueCollection
        'Create new collection of values
        Dim values As SettingsPropertyValueCollection = New SettingsPropertyValueCollection()

        'Iterate through the settings to be retrieved
        For Each setting As SettingsProperty In props

            Dim value As SettingsPropertyValue = New SettingsPropertyValue(setting)
            value.IsDirty = False
            value.SerializedValue = GetValue(setting)
            values.Add(value)
        Next
        Return values
    End Function

    Private m_SettingsXML As XmlDocument = Nothing

    Private ReadOnly Property SettingsXML As XmlDocument
        Get
            'If we dont hold an xml document, try opening one.  
            'If it doesnt exist then create a new one ready.
            If m_SettingsXML Is Nothing Then
                m_SettingsXML = New XmlDocument

                Try
                    m_SettingsXML.Load(Path.Combine(GetAppSettingsPath, GetAppSettingsFilename))
                Catch ex As Exception
                    'Create new document
                    Dim dec As XmlDeclaration = m_SettingsXML.CreateXmlDeclaration("1.0", "utf-8", String.Empty)
                    m_SettingsXML.AppendChild(dec)

                    Dim nodeRoot As XmlNode

                    nodeRoot = m_SettingsXML.CreateNode(XmlNodeType.Element, SETTINGSROOT, "")
                    m_SettingsXML.AppendChild(nodeRoot)
                End Try
            End If

            Return m_SettingsXML
        End Get
    End Property

    Private Function GetValue(setting As SettingsProperty) As String
        Dim ret As String = ""

        Try
            If IsRoaming(setting) Then
                ret = SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & setting.Name).InnerText
            Else
                ret = SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name & "/" & setting.Name).InnerText
            End If

        Catch ex As Exception
            If Not setting.DefaultValue Is Nothing Then
                ret = setting.DefaultValue.ToString
            Else
                ret = ""
            End If
        End Try

        Return ret
    End Function

    Private Sub SetValue(propVal As SettingsPropertyValue)

        Dim MachineNode As XmlElement
        Dim SettingNode As XmlElement

        'Determine if the setting is roaming.
        'If roaming then the value is stored as an element under the root
        'Otherwise it is stored under a machine name node 
        Try
            If IsRoaming(propVal.Property) Then
                SettingNode = DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & propVal.Name), XmlElement)
            Else
                SettingNode =
                    DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name & "/" & propVal.Name),
                               XmlElement)
            End If
        Catch ex As Exception
            SettingNode = Nothing
        End Try

        'Check to see if the node exists, if so then set its new value
        If Not SettingNode Is Nothing Then
            SettingNode.InnerText = propVal.SerializedValue.ToString
        Else
            If IsRoaming(propVal.Property) Then
                'Store the value as an element of the Settings Root Node
                SettingNode = SettingsXML.CreateElement(propVal.Name)
                SettingNode.InnerText = propVal.SerializedValue.ToString
                SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(SettingNode)
            Else
                'Its machine specific, store as an element of the machine name node,
                'creating a new machine name node if one doesnt exist.
                Try
                    MachineNode = DirectCast(SettingsXML.SelectSingleNode(SETTINGSROOT & "/" & My.Computer.Name),
                                             XmlElement)
                Catch ex As Exception
                    MachineNode = SettingsXML.CreateElement(My.Computer.Name)
                    SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode)
                End Try

                If MachineNode Is Nothing Then
                    MachineNode = SettingsXML.CreateElement(My.Computer.Name)
                    SettingsXML.SelectSingleNode(SETTINGSROOT).AppendChild(MachineNode)
                End If

                SettingNode = SettingsXML.CreateElement(propVal.Name)
                SettingNode.InnerText = propVal.SerializedValue.ToString
                MachineNode.AppendChild(SettingNode)
            End If
        End If
    End Sub

    Private Function IsRoaming(prop As SettingsProperty) As Boolean
        'Determine if the setting is marked as Roaming
        For Each d As DictionaryEntry In prop.Attributes
            Dim a As Attribute = DirectCast(d.Value, Attribute)
            If TypeOf a Is SettingsManageabilityAttribute Then
                Return True
            End If
        Next
        Return False
    End Function
End Class
