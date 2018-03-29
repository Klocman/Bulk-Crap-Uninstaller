using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace PortableSettingsProvider
{
    /// <summary>
    /// License: The Code Project Open License (CPOL) 1.02
    /// 18 Oct 2007 - CodeChimp - Public VB release
    /// Mar 26, 2016 - HakanL - Converted project to C#, cleanup
    /// 2010-2017 - Marcin Szeniak - Bugfixes, cleanup, improvements
    /// </summary>
    public class PortableSettingsProvider : SettingsProvider
    {
        //XML Root Node name
        private const string SettingsRootName = "Settings";

        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }

                var fi = new FileInfo(Application.ExecutablePath);
                return fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
            }
            //Do nothing
            set { }
        }
        
        private XmlDocument _settingsXml;
        private XmlDocument SettingsXml
        {
            get
            {
                //If we dont hold an xml document, try opening one.  
                //If it doesnt exist then create a new one ready.
                if (_settingsXml == null)
                {
                    _settingsXml = new XmlDocument();

                    try
                    {
                        _settingsXml.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        //Create new document
                        var dec = _settingsXml.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        _settingsXml.AppendChild(dec);

                        var nodeRoot = _settingsXml.CreateNode(XmlNodeType.Element, SettingsRootName, "");
                        _settingsXml.AppendChild(nodeRoot);
                    }
                }

                return _settingsXml;
            }
        }

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(ApplicationName, col);
        }

        public virtual string GetAppSettingsPath()
        {
            //Used to determine where to store the settings
            var fi = new FileInfo(Application.ExecutablePath);
            return fi.DirectoryName;
        }

        public virtual string GetAppSettingsFilename()
        {
            //Used to determine the filename to store the settings
            return ApplicationName + ".settings";
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            //Iterate through the settings to be stored
            //Only dirty settings are included in propvals, and only ones relevant to this provider
            foreach (SettingsPropertyValue propval in propvals)
            {
                SetValue(propval);
            }

            try
            {
                SettingsXml.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
            }
            catch (Exception)
            {
                //Ignore if cant save, device been ejected
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context,
            SettingsPropertyCollection props)
        {
            //Create new collection of values
            var values = new SettingsPropertyValueCollection();

            //Iterate through the settings to be retrieved

            foreach (SettingsProperty setting in props)
            {
                var value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = GetValue(setting);
                values.Add(value);
            }
            return values;
        }

        private string GetValue(SettingsProperty setting)
        {
            try
            {
                if (IsRoaming(setting))
                {
                    return SettingsXml.SelectSingleNode(SettingsRootName + "/" + setting.Name)?.InnerText
                           ?? GetDefaultValue(setting);
                }

                return SettingsXml.SelectSingleNode(SettingsRootName + "/" + Environment.MachineName
                                                    + "/" + setting.Name)?.InnerText ?? GetDefaultValue(setting);
            }
            catch (Exception)
            {
                return GetDefaultValue(setting);
            }
        }

        private static string GetDefaultValue(SettingsProperty setting)
        {
            return setting.DefaultValue?.ToString() ?? string.Empty;
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            if (propVal == null)
                throw new ArgumentNullException(nameof(propVal));
            if (propVal.SerializedValue == null)
                throw new ArgumentNullException(nameof(propVal.SerializedValue));

            XmlElement settingNode;

            //Determine if the setting is roaming.
            //If roaming then the value is stored as an element under the root
            //Otherwise it is stored under a machine name node 
            try
            {
                if (IsRoaming(propVal.Property))
                {
                    settingNode = (XmlElement) SettingsXml.SelectSingleNode(SettingsRootName + "/" + propVal.Name);
                }
                else
                {
                    settingNode = (XmlElement) SettingsXml.SelectSingleNode(SettingsRootName + "/" + Environment.MachineName
                                                                            + "/" + propVal.Name);
                }
            }
            catch (Exception)
            {
                settingNode = null;
            }

            //Check to see if the node exists, if so then set its new value
            if (settingNode != null)
            {
                settingNode.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                if (IsRoaming(propVal.Property))
                {
                    //Store the value as an element of the Settings Root Node
                    settingNode = SettingsXml.CreateElement(propVal.Name);
                    settingNode.InnerText = propVal.SerializedValue.ToString();
                    SettingsXml.SelectSingleNode(SettingsRootName).AppendChild(settingNode);
                }
                else
                {
                    //Its machine specific, store as an element of the machine name node,
                    //creating a new machine name node if one doesnt exist.
                    XmlElement machineNode;
                    try
                    {
                        machineNode =
                            (XmlElement) SettingsXml.SelectSingleNode(SettingsRootName + "/" + Environment.MachineName);
                    }
                    catch (Exception)
                    {
                        machineNode = SettingsXml.CreateElement(Environment.MachineName);
                        SettingsXml.SelectSingleNode(SettingsRootName).AppendChild(machineNode);
                    }

                    if (machineNode == null)
                    {
                        machineNode = SettingsXml.CreateElement(Environment.MachineName);
                        SettingsXml.SelectSingleNode(SettingsRootName).AppendChild(machineNode);
                    }

                    settingNode = SettingsXml.CreateElement(propVal.Name);
                    settingNode.InnerText = propVal.SerializedValue.ToString();
                    machineNode.AppendChild(settingNode);
                }
            }
        }

        private bool IsRoaming(SettingsProperty prop)
        {
            //Determine if the setting is marked as Roaming
            return prop.Attributes.Cast<DictionaryEntry>().Select(x => x.Value)
                .OfType<SettingsManageabilityAttribute>().Any();

            /*foreach (DictionaryEntry d in prop.Attributes)
            {
                Attribute a = (Attribute)d.Value;
                if (a is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;*/
        }
    }
}