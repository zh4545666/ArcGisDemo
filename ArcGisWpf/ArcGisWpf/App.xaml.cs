using ESRI.ArcGIS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ArcGisWpf
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {

                MessageBox.Show("Unable to bind to ArcGIS runtime. Application will be shut down.");
                return;

            }
            ESRI.ArcGIS.esriSystem.IAoInitialize ao = new ESRI.ArcGIS.esriSystem.AoInitialize();
            ao.Initialize(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced);

            base.OnStartup(e);
        }
    }
}
