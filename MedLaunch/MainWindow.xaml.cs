﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.IO;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Migrations;
using MedLaunch.Models;
using MedLaunch.Classes;
using Asnitech.Launch.Common;
using Ookii.Dialogs;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using MahApps.Metro.Controls.Dialogs;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json;
using System.Threading;
using Medlaunch.Classes;
using System.Net;
using MedLaunch.Classes.MobyGames;
using MedLaunch.Classes.MasterScraper;
using MedLaunch.Classes.TheGamesDB;
using MedLaunch.Classes.Scraper;
using MedLaunch.Classes.Scraper.ReplacementDocs;
using MahApps.Metro;
using MedLaunch.Classes.Input;
using MedLaunch.Classes.GamesLibrary;
using System.Collections.ObjectModel;
using MedLaunch.Classes.Scraper.DAT.TOSEC.Models;
using MedLaunch.Classes.Scraper.DAT.NOINTRO.Models;
using MedLaunch.Classes.Scraper.DAT.Models;
using MedLaunch.Classes.Scraper.DAT.TRURIP.Models;
using MedLaunch.Classes.Scraper.DAT.REDUMP.Models;
using MedLaunch.Classes.VisualHandlers;
using Newtonsoft.Json.Linq;
using MedLaunch.Classes.IO;
using MahApps.Metro.SimpleChildWindow;
using MedLaunch.Classes.Controls.VirtualDevices;
using MedLaunch.Classes.Controls.InputManager;

namespace MedLaunch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<DataGridGamesView> dg { get; set; }
        public bool SettingsDirtyFlag { get; set; }
        public string LaunchString { get; set; }

        public DeviceDefinition ControllerDefinition { get; set; }

        public MainWindow()
        {
            // make sure class libraries are built
            //Asnitech.Launch.Common.Startup.Start();

            // initialise directories if they do not exist
            //SetupDirectories.Go();

            // check the database version if it exists - run the update/migration procedure if necessary
            //DbMigration.CheckVersions();

            // initialise SQLite db if it does not already exist
            /*
            using (var context = new MyDbContext())
            {
                context.Database.EnsureCreated();
                // populate stock data 
                DbEF.InitialSeed();
                context.SaveChanges();
            }
            */
            

            InitializeComponent();


            MainWindow mw = this;
            //this.WindowState = WindowState.Normal;

            // set settings dirty flag
            SettingsDirtyFlag = false; // not dirty - do not save any settings

            // instantiate ScrapedContent Object
            GamesLibraryScrapedContent ScrapedData = new GamesLibraryScrapedContent();

            // doubleclick handler for gui_zoom control
            //gui_zoom.MouseDoubleClick += new MouseButtonEventHandler(RestoreScalingFactor);
            dpZoomSlider.Visibility = Visibility.Collapsed;
            //gui_zoom.Value = Convert.ToDouble(gui_zoom_combo.SelectedValue);


            // set window size
            this.Height = 768;
            this.Width = 1366;

            // check workspace size, if mahapps resolution is too big - go full screen
            int wWidth = Convert.ToInt32(SystemParameters.WorkArea.Width);
            int wHeight = Convert.ToInt32(SystemParameters.WorkArea.Height);

            if (this.Height > wHeight || this.Width > wWidth)
            {
                // maximise window
                this.WindowState = WindowState.Maximized;
            }


            // get application version
            string appVersion = Versions.ReturnApplicationVersion();
            /*
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string versionMajor = fvi.ProductMajorPart.ToString();
            string versionMinor = fvi.ProductMinorPart.ToString();
            string versionBuild = fvi.ProductBuildPart.ToString();
            string versionPrivate = fvi.ProductPrivatePart.ToString();
            
            string fVersion = fvi.FileVersion;
            */

            // set title
            string linkTimeLocal = (Assembly.GetExecutingAssembly().GetLinkerTime()).ToString("yyyy-MM-dd HH:mm:ss");
            this.Title = "MedLaunch (v" + appVersion + ") - Windows Front-End for Mednafen"; // (v" + Versions.CompatibleMednafenBranch() + " only)";

            rightMenuLabel.Text = "(Compatible Mednafen v" + Versions.CompatibleMednafenBranch() + " only)";
            //this.Title = "MedLaunch - Windows Front-End for Mednafen (v" + versionMajor + "." + versionMinor + "." + versionBuild + "." + versionPrivate + ")"; // - Built: "+ linkTimeLocal;
            //this.Title = "MedLaunch - Windows Front-End for Mednafen (" + fVersion + ")";

            // Startup checks

            // is DB path to Mednafen set and working? If not force user to select it
            Paths.MedPathRoutine(btnPathMednafen, tbPathMednafen);

            // instatiate GamesList
            //App _App = ((App)Application.Current);
            //_App.GamesList = new Classes.GamesLibrary.GameListBuilder();

            

            // ensure 'show all' filter is checked on startup
            btnShowAll.IsChecked = true;

            //App _App = ((App)Application.Current);
            //dg = _App.GamesList.FilteredSet;

            // load globalsettings for front page
            GlobalSettings.LoadGlobalSettings(chkEnableNetplay, chkEnableSnes_faust, chkEnablePce_fast, gui_zoom_combo, chkMinToTaskbar, chkHideSidebar,
               chkAllowBanners, chkAllowBoxart, chkAllowScreenshots, chkAllowFanart, chkPreferGenesis, chkAllowManuals, chkAllowMedia, chkSecondaryScraperBackup,
               rbGDB, rbMoby, slScreenshotsPerHost, slFanrtsPerHost, chkAllowUpdateCheck, chkBackupMednafenConfig, chkSaveSysConfigs, comboImageTooltipSize, chkLoadConfigsOnStart, chkEnableConfigToolTips,
               chkshowGLYear, chkshowGLESRB, chkshowGLCoop, chkshowGLDeveloper, chkshowGLPublisher, chkshowGLPlayers, chkEnableClearCacheOnExit, chkrememberSysWinPositions);
            //gui_zoom.Value = Convert.ToDouble(gui_zoom_combo.SelectedValue);
            GlobalSettings gs = GlobalSettings.GetGlobals();
            mainScaleTransform.ScaleX = Convert.ToDouble(gs.guiZoom);
            mainScaleTransform.ScaleY = Convert.ToDouble(gs.guiZoom);




            // load netplay settings for netplay page
            ConfigNetplaySettings.LoadNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
            //ConfigNetplaySettings.LoadNetplaySettings();

            // load path settings for paths page
            Paths.LoadPathSettings(tbPathMednafen, tbPathGb, tbPathGba, tbPathGg, tbPathLynx, tbPathMd, tbPathNes, tbPathSnes, tbPathNgp, tbPathPce, tbPathPcfx, tbPathSms, tbPathVb, tbPathWswan); // tbPathPsx, tbPathSs);

            // force location of Mednafen directory
            //var controller = this.ShowProgressAsync("Please wait...", "Progress Message");

            // initialise servers area

            // DbEF.PopulateServersCombo(cbServers);
            /*
            ConfigServerSettings.PopulateServersRadio(rbSrv01);
            ConfigServerSettings.PopulateServersRadio(rbSrv02);
            ConfigServerSettings.PopulateServersRadio(rbSrv03);
            ConfigServerSettings.PopulateServersRadio(rbSrv04);
            ConfigServerSettings.PopulateCustomServer(tbServerDesc, tbHostname, slServerPort, tbPassword, tbGameKey);
            ConfigServerSettings.GetSelectedServerCheckbox(rbSrv01, rbSrv02, rbSrv03, rbSrv04, rbSrvCustom);
            */

            SettingsVisualHandler.PopulateServers(lvServers);
            SettingsVisualHandler.ServerSettingsInitialButtonHide();


            // Config Tab

            // Set radiobutton states
            foreach (UIElement element in ConfigSelectorWrapPanel.Children)
            {
                if (element is RadioButton)
                {
                    // Set the radiobutton state (enabled or disabled)
                    //MessageBoxResult result = MessageBox.Show((element as RadioButton).Name.ToLower());
                    ConfigBaseSettings.SetButtonState(element as RadioButton);
                }
            }

            // ensure a visable config filter button is selected
            /*
            List<RadioButton> _filterButtons = UIHandler.GetLogicalChildCollection<RadioButton>(wpConfigLeftPane);
            foreach (RadioButton rb in _filterButtons)
            {
                if (rb.Visibility == Visibility.Visible)
                {
                    rb.IsChecked = true;
                    rb.IsChecked = false;
                    rb.IsChecked = true;
                }
            }
            */

            /*
            btnConfigNes.IsChecked = true;
            btnConfigNes.IsChecked = false;
            btnConfigNes.IsChecked = true;
            */

            // controls tab
            btnAllControls.IsChecked = true;
            //btnControlNes.IsChecked = true;

            // settings tab
            btnAllSettings.IsChecked = true;
            SettingsVisualHandler svh = new SettingsVisualHandler();
            // get the button state and active/deativate required panels
            svh.SetFilter();
            // load all settings
            SettingsHandler sh = new SettingsHandler();
            sh.LoadAllSettings();

            // load all MednafenPath settings
            ConfigBaseSettings.LoadMednafenPathValues(spMedPathSettings);
            ConfigBaseSettings.LoadBiosPathValues(spSysBiosSettings);




            // load mednafen help page
            wb.Navigate("http://mednafen.fobby.net/");


            /* hide certain controls (whilst they are being developed) */

            // rescan all disk systems button
            btnRescanDisks.Visibility = Visibility.Collapsed;

            // Rescan specific disk system menu item
            ScanDisks13.Visibility = Visibility.Collapsed;
            ScanDisks9.Visibility = Visibility.Collapsed;
            ScanDisks18.Visibility = Visibility.Collapsed;
            ScanDisks8.Visibility = Visibility.Collapsed;

            // settings buttons and borders
            //btnMednafenPaths.Visibility = Visibility.Collapsed;
            //btnSystemBios.Visibility = Visibility.Collapsed;
            //btnEmulator.Visibility = Visibility.Collapsed;
            //brdMednafenPaths.Visibility = Visibility.Collapsed;
            /// brdSystemBios.Visibility = Visibility.Collapsed;
            //brdEmulator.Visibility = Visibility.Collapsed;

            wb.Navigated += new NavigatedEventHandler(wb_Navigated);

            // games library
            GamesLibraryVisualHandler.UpdateSidebar();

            // load expander states
            GamesLibraryVisualHandler.LoadExpanderStates();
            // enable events once expander states have been set
            /*
            List < Expander > exp = GamesLibraryVisualHandler.GetExpanderControls();
            foreach (Expander e in exp)
            {
                e.Collapsed += new RoutedEventHandler(GamesLibraryExpanderSaveLayout);
                e.Expanded += new RoutedEventHandler(GamesLibraryExpanderSaveLayout);
            }
            */

            // GameScraper.GetPlatformGames(4924);

            // about tab
            lblVersion.Visibility = Visibility.Collapsed;
            lblDate.Visibility = Visibility.Collapsed;
            tbNotes.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            lbl1.Visibility = Visibility.Collapsed;
            lbl2.Visibility = Visibility.Collapsed;
            lbl3.Visibility = Visibility.Collapsed;
            lbl4.Visibility = Visibility.Collapsed;
            lblNoUpdate.Visibility = Visibility.Collapsed;

            /*
            // change mouse-over cursor on buttons to hand
            foreach (Button btn in FindVisualChildren<Button>(this))
            {
                btn.Cursor = Cursors.Hand;
            }
            // change mouse-over cursor on checkboxes to hand
            foreach (CheckBox cb in FindVisualChildren<CheckBox>(this))
            {
                cb.Cursor = Cursors.Hand;
            }
            // change mouse-over cursor on togglebutton to hand
            foreach (ToggleButton btn in FindVisualChildren<ToggleButton>(this))
            {
                btn.Cursor = Cursors.Hand;
            }
            */

            btnReLink.Visibility = Visibility.Collapsed;

            // enable tooltips if neccesary
            if (gs.enableConfigToolTips == true)
            {
                ConfigToolTips.SetToolTips(1);
            }

            //SettingsDirtyFlag = true; // now its safe to save settings as all content is loaded


            // initialise input class
            Input.Initialize(this);

        }

        private MetroWindow colorSchemeChanger;

        private void btnChangeColorScheme_Click(object sender, RoutedEventArgs e)
        {
            if (colorSchemeChanger != null)
            {
                colorSchemeChanger.Activate();
                return;
            }

            colorSchemeChanger = new AccentStyleWindow();
            colorSchemeChanger.Owner = this;
            colorSchemeChanger.Closed += (o, args) => colorSchemeChanger = null;
            colorSchemeChanger.Left = this.Left + this.ActualWidth / 2.0;
            colorSchemeChanger.Top = this.Top + this.ActualHeight / 2.0;
            colorSchemeChanger.Show();
        }

        void RestoreScalingFactor(object sender, MouseButtonEventArgs args)
        {

            ((Slider)sender).Value = 1.0;
        }
        private void gui_zoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double value = Convert.ToDouble((sender as ComboBox).SelectedValue);
            //MessageBox.Show(value.ToString());
            //gui_zoom.Value = value;
            if (Convert.ToDouble(value) > 0)
            {
                GlobalSettings.UpdateGuiZoom(value);
            }

            // now update zoom
            ScaleTransform st = (ScaleTransform)this.FindName("mainScaleTransform");
            st.ScaleX = value;
            st.ScaleY = value;

        }


        // web browser

        void wb_Navigated(object sender, NavigationEventArgs e)
        {
            SetSilent(wb, true); // make it silent
        }

        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }


        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }

        private void txtUrl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                wb.Navigate(txtUrl.Text);
        }

        private void wb_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            txtUrl.Text = e.Uri.OriginalString;

        }


        private void BrowseBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((wb != null) && (wb.CanGoBack));
        }

        private void BrowseBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wb.GoBack();
        }

        private void BrowseForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ((wb != null) && (wb.CanGoForward));
        }

        private void BrowseForward_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wb.GoForward();
        }

        private void GoToPage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoToPage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            wb.Navigate(txtUrl.Text);
        }

        private void btnWbMednafenDoc_Click(object sender, RoutedEventArgs e)
        {
            wb.Navigate(new Uri("http://mednafen.fobby.net/documentation/", UriKind.RelativeOrAbsolute));
        }

        private void btnWbMednafenForum_Click(object sender, RoutedEventArgs e)
        {
            wb.Navigate(new Uri("http://forum.fobby.net/index.php?t=i&", UriKind.RelativeOrAbsolute));
        }

        private void btnWbMedLaunch_Click(object sender, RoutedEventArgs e)
        {
            wb.Navigate(new Uri("http://medlaunch.asnitech.co.uk/", UriKind.RelativeOrAbsolute));
        }

        private void btnWbMednafenHome_Click(object sender, RoutedEventArgs e)
        {
            wb.Navigate(new Uri("http://mednafen.fobby.net/", UriKind.RelativeOrAbsolute));
        }

        private async void RescanSystemDisks(int sysId)
        {
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scanning",
                AnimateShow = true,
                AnimateHide = true

            };

            var controller = await this.ShowProgressAsync("Scanning Disk Directories", "Determining Paths and Counting Files...", settings: mySettings);
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            await Task.Delay(100);

            btnFavorites.IsChecked = true;
            btnShowAll.IsChecked = true;

            await controller.CloseAsync();
        }

        private void RescanDisks(object sender, RoutedEventArgs e)
        {
            RescanSystemDisks(0);
        }

        private async void ScrapeGetAllPlatformGames_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource cs = new CancellationTokenSource();
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scraping",
                AnimateShow = false,
                AnimateHide = false,
            };

            var controller = await this.ShowProgressAsync("MedLaunch - Getting Basic Games List From thegamesdb.net", "", settings: mySettings);
            controller.SetCancelable(true);
            await Task.Delay(100);
            await Task.Run(() =>
            {
                Task.Delay(1);
                //List<GDBPlatformGame> gs = GameScraper.DatabasePlatformGamesImport(controller);
                GDBScraper.ScrapeBasicGamesList(controller);
                /*
                if (!controller.IsCanceled)
                {
                    //GDBPlatformGame.SaveToDatabase(gs);       // disabled for the moment - working with flat json files
                    // save to json file
                    controller.SetMessage("Saving to file...");
                    // set file path
                    string filePath = @"..\..\Data\System\TheGamesDB.json";
                    //  dump file
                    string json = JsonConvert.SerializeObject(gs, Formatting.Indented);
                    File.WriteAllText(filePath, json);
                }
                */

            });
            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("GDB Master Games List Download", "Operation Cancelled");
            }
            else
            {
                await this.ShowMessageAsync("GDB Master Games List Download", "Scanning Completed");
            }
        }

        private async void btnScrapeAllGameInfo_Click(object sender, RoutedEventArgs e)
        {
            // ensure directory is created
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Data\Settings");

            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scraping",
                AnimateShow = false,
                AnimateHide = false,
            };

            var controller = await this.ShowProgressAsync("MedLaunch - Downloading all game info (will take a while)", "", settings: mySettings);
            controller.SetCancelable(true);
            await Task.Delay(100);
            await Task.Run(() =>
            {
                Task.Delay(1);
                // get list of all platformgames in the database
                controller.SetMessage("Unumerating PlatForm games from database...");
                App app = ((App)Application.Current);
                List<ScraperMaster> games = app.ScrapedData.MasterPlatformList;
                int gameCount = games.Count;
                int i = 0;

                foreach (ScraperMaster g in games)
                {
                    i++;
                    WebOps wo = new WebOps();
                    wo.Params = "/GetGame.php?id=" + g.GamesDbId;

                }



            });
            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("MedLaunch - GameScraper", "Operation Cancelled");
            }
            else
            {
                await this.ShowMessageAsync("MedLaunch - GameScraper", "Downloading Completed");
            }
        }


        private async void RescanSystemRoms(int sysId)
        {

            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scanning",
                AnimateShow = true,
                AnimateHide = true
            };

            var controller = await this.ShowProgressAsync("Scanning ROM Directories", "Determining Paths and Counting Files...", settings: mySettings);
            controller.SetCancelable(true);
            //controller.SetIndeterminate();

            await Task.Delay(100);

            string output = "";
            int addedStats = 0;
            int updatedStats = 0;
            int untouchedStats = 0;
            int hiddenStats = 0;

            GameScanner rs = new GameScanner();


            await Task.Delay(100);



            List<GSystem> scanRoms = new List<GSystem>();
            if (sysId == 0)
            {
                /* scan of all roms has been selected */

                // mark all roms in database as hidden where the system path is not set or if path no longer exists
                foreach (var hs in rs.Systems)
                {
                    string path = rs.GetPath(hs.systemId);
                    if (path == "" || path == null || !Directory.Exists(path))
                    {

                        // No path returned or path is not valid - Mark existing games in Db as hidden
                        rs.MarkAllRomsAsHidden(hs.systemId);
                        hiddenStats += rs.HiddenStats;
                        rs.HiddenStats = 0;
                    }
                }

                scanRoms = rs.RomSystemsWithPaths;
            }
            else
            {
                /* only one system has been selected for scanning */

                // mark all roms in database as hidden for this system path is not set or if path no longer exists                
                string path = rs.GetPath(sysId);
                if (path == "" || path == null || !Directory.Exists(path))
                {
                    // No path returned or path is not valid - Mark existing games in Db as hidden
                    rs.MarkAllRomsAsHidden(sysId);
                    hiddenStats += rs.HiddenStats;
                    rs.HiddenStats = 0;
                }


                scanRoms = (from s in rs.RomSystemsWithPaths
                            where s.systemId == sysId
                            select s).ToList();
            }

            // check whether scanroms is null
            if (scanRoms.Count > 0)
            {
                // start the operations on a different thread
                await Task.Run(() =>
                {
                    // data has been returned

                    // how many systems returned
                    int sysCount = scanRoms.Count;
                    controller.Minimum = 0;
                    controller.Maximum = sysCount;
                    int progress = 0;

                    // iterate through each system that has a system ROM path set
                    foreach (var s in scanRoms)
                    {
                        if (controller.IsCanceled)
                        {
                            break;
                        }
                        //MessageBoxResult result2 = MessageBox.Show(s.systemId.ToString());

                        // start scanning
                        controller.SetTitle("Starting " + s.systemName + " (" + s.systemCode + ") Scan");
                        Task.Delay(100);
                        //output += "Scanning....";
                        controller.SetMessage(output);

                        progress++;
                        controller.SetProgress(progress);

                        // Start ROM scan for this system
                        rs.BeginRomImport(s.systemId, controller);

                        //output += ".....Completed\n\n";

                        // update totals
                        addedStats += rs.AddedStats;
                        updatedStats += rs.UpdatedStats;
                        untouchedStats += rs.UntouchedStats;
                        hiddenStats += rs.HiddenStats;

                        //output += rs.AddedStats + " ROMs Added\n" + rs.UpdatedStats + " ROMs Updated\n" + rs.UntouchedStats + " ROMs Skipped\n" + rs.HiddenStats + " ROMs Missing (marked as hidden)\n";
                        //controller.SetMessage(output);

                        // reset class totals
                        rs.AddedStats = 0;
                        rs.UpdatedStats = 0;
                        rs.UntouchedStats = 0;
                        rs.HiddenStats = 0;

                         Task.Delay(200);
                    }
                });

            }
            else
            {
                // No systems returned
                controller.SetTitle("No ROM systems with valid paths found");
                controller.SetMessage("No GameSystem with valid path was found\n Please make sure there is a valid path set for this system");

            }

            await Task.Run(() =>
            {
                if (!controller.IsCanceled)
                {
                    controller.SetMessage(output + "\nUpdating Database");
                    rs.SaveToDatabase();
                }

            });



            await Task.Delay(100);



            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("RomScanner", "Operation Cancelled");
            }
            else
            {
                await this.ShowMessageAsync("Scanning Completed", "Totals:\n\nROMs Added: " + addedStats + "\nROMs Updated: " + updatedStats + "\nROMs Skipped: " + untouchedStats + "\nROMs Marked as Hidden: " + hiddenStats);
            }

            //Update list
            // ensure 'show all' filter is checked on startup
            //btnFavorites.IsChecked = true;
            //btnShowAll.IsChecked = true;

            // update data is changes have been made
            if (addedStats > 0 || updatedStats > 0 || hiddenStats > 0)
                GameListBuilder.UpdateFlag();

            // refresh library view
            GamesLibraryVisualHandler.RefreshGamesLibrary();


            /*

            // get systems
            List<GameSystem> systems = RomScanner.GetSystems();
            List<Rom> romSystem = new List<Rom>();
            

            // iterate through each system and check if ROM path exists
            foreach (var s in systems)
            {
                string path = RomScanner.GetPath(s.systemId);
                if (path == "" || path == null)
                {
                    //do nothing
                }
                else
                {
                    //MessageBoxResult result = MessageBox.Show("System: " + s.systemName + " - Path: " + path, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    Rom r = new Rom();
                    r.gameSystem = s;
                    r.path = path;
                    romSystem.Add(r);
                }
            }

            // we now have a List<Rom> that contains all systems that have a filesystem directory path set
            // iterate through each path and count the number of files

            int totalFiles = 0;
            int romsInserted = 0;
            int romsUpdated = 0;
            int romsSkipped = 0;
            foreach (var p in romSystem)
            {
                int systemFiles = RomScanner.CountFiles(p.path);
                totalFiles += systemFiles;
            }

            controller.SetMessage(totalFiles + " files found across all ROM directories");

            await Task.Delay(500);

            foreach (var item in romSystem)
            {
                // actually scan through ROMS
                string SystemName = item.gameSystem.systemName;
                controller.SetTitle("Importing " + SystemName + " Roms");

                // get a collection of files from each ROM directory
                var files = RomScanner.GetFiles(item.path);

                // iterate through each file
                foreach (var file in files)
                {
                    // get the relative path
                    string relPath = PathUtil.GetRelativePath(item.path, file);

                    // get just the filename
                    string fileName = System.IO.Path.GetFileName(file);

                    // get just the extension
                    string extension = System.IO.Path.GetExtension(file);

                    // get rom name wihout extension
                    string romName = fileName.Replace(extension, "");

                    //MessageBoxResult result = MessageBox.Show(romName, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    // attempt to add Rom to database
                    int addRom = RomScanner.AddGame(item, file, relPath, fileName, extension, romName);

                    string romProcess = "";

                    switch (addRom)
                    {
                        case 1:
                            // Rom inserted
                            romsInserted++;
                            romProcess = "Added to Database";
                            break;
                        case 2:
                            // Rom updated
                            romsUpdated++;
                            romProcess = "Updated in Database";
                            break;
                        default:
                            // Rom skipped
                            romsSkipped++;
                            romProcess = "Has Been Skipped";
                            break;               
                    }

                    controller.SetMessage("ROM: " + romName + " " + romProcess);

                }
                await Task.Delay(100);
            }

            /*
            double i = 0.0;
            while (i < Convert.ToDouble(totalFiles + 1))
            {
                //double val = (i / 100.0) * totalFiles;
                double val = (100 / totalFiles) * i / 100;
                controller.SetProgress(val);
                
                controller.SetMessage("Scanning " +  + i + "...");

                if (controller.IsCanceled)
                    break; //canceled progressdialog auto closes.

                i += 1.0;

                await Task.Delay(2000);
            }
            

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("The operation was cancelled!", romsInserted +  " ROMS have been added \n" + romsUpdated + " ROMS have been updated \n" + romsSkipped + " ROMS have been skipped");
            }
            else
            {
                await this.ShowMessageAsync("Operation completed", romsInserted + " ROMS have been added \n" + romsUpdated + " ROMS have been updated \n" + romsSkipped + " ROMS have been skipped");
            }
            */
            //Update list
            // ensure 'show all' filter is checked on startup
            //btnFavorites.IsChecked = true;
            //btnShowAll.IsChecked = true;

            // refresh library view
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void RescanRoms(object sender, RoutedEventArgs e)
        {
            RescanSystemRoms(0);
        }





        /*********** 
         * EVENTS
         * *********/






        // Misc
        private void System_Image_Loaded(object sender, RoutedEventArgs e)
        {

        }

        // Game filter buttons
        private void btnShowAll_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(1);
            // load datagrid
            DbEF.GetGames(dgGameList, 0, tbFilterDatagrid.Text);
           // dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnFavorites_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(2);
            // load datagrid
            DbEF.GetGames(dgGameList, -1, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnUnscraped_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(3);
            // load datagrid
            DbEF.GetGames(dgGameList, -100, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnNes_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(4);
            // load datagrid
            DbEF.GetGames(dgGameList, 11, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnSnes_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(5);
            // load datagrid
            DbEF.GetGames(dgGameList, 12, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnSms_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(6);
            // load datagrid
            DbEF.GetGames(dgGameList, 10, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnMd_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(7);
            // load datagrid
            DbEF.GetGames(dgGameList, 4, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnSs_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(16);
            // load datagrid
            DbEF.GetGames(dgGameList, 13, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnPsx_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(17);
            // load datagrid
            DbEF.GetGames(dgGameList, 9, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnPce_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(8);
            // load datagrid
            DbEF.GetGames(dgGameList, 7, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }
        private void btnPcecd_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(18);
            // load datagrid
            DbEF.GetGames(dgGameList, 18, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnPcfx_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(19);
            // load datagrid
            DbEF.GetGames(dgGameList, 8, tbFilterDatagrid.Text);
           // dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnVb_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(9);
            // load datagrid
            DbEF.GetGames(dgGameList, 14, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnNgp_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(10);
            // load datagrid
            DbEF.GetGames(dgGameList, 6, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnWswan_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(11);
            // load datagrid
            DbEF.GetGames(dgGameList, 15, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnGb_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(12);
            // load datagrid
            DbEF.GetGames(dgGameList, 1, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnGba_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(13);
            // load datagrid
            DbEF.GetGames(dgGameList, 2, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnGg_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(14);
            // load datagrid
            DbEF.GetGames(dgGameList, 5, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        private void btnLynx_Checked(object sender, RoutedEventArgs e)
        {
            // set column visibility
            GamesLibraryVisualHandler.SetColumnVisibility(15);
            // load datagrid
            DbEF.GetGames(dgGameList, 3, tbFilterDatagrid.Text);
            //dg = ((App)Application.Current).GamesList.FilteredSet;
        }

        // game filter context menu events
        private void ScanRoms_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = 0;
            if (menuName.StartsWith("ScanRoms"))
            {
                sysId = Convert.ToInt32(menuName.Replace("ScanRoms", ""));
            }
            if (menuName.StartsWith("MenuScanRoms"))
            {
                sysId = Convert.ToInt32(menuName.Replace("MenuScanRoms", ""));
            }

            RescanSystemRoms(sysId);
        }

        private void RemoveRoms_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("RemoveRoms", ""));
            GameScanner.RemoveRoms(sysId);
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void ScanDisks_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("ScanDisks", ""));
        }

        private void RemoveDisks_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("RemoveDisks", ""));
            GameScanner.RemoveDisks(sysId);
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void ManualAddGame_Click(object sender, RoutedEventArgs e)
        {
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("ManualAddGame", ""));
            GameScanner gs = new GameScanner();
            gs.BeginManualImport(sysId);
            // refresh library view
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void RemoveAllGames_Click(object sender, RoutedEventArgs e)
        {
            GameScanner.RemoveAllGames();
        }

        private void ScrapeFavorites_Click(object sender, RoutedEventArgs e)
        {            
            ScraperHandler.ScrapeGames(0, ScrapeType.Favorites);
        }

        private void ReScrapeFavorites_Click(object sender, RoutedEventArgs e)
        {
            ScraperHandler.ScrapeGames(0, ScrapeType.RescrapeFavorites);
        }

        private void ScrapeGamesMultiple_Click(object sender, RoutedEventArgs e)
        {
            // get number of selected rows
            int numOfRows = dgGameList.SelectedItems.Count;
            List<DataGridGamesView> games = new List<DataGridGamesView>();

            if (numOfRows == 0)
                return;
            else if (numOfRows == 1)
            {
                // single row selected
                games.Add((DataGridGamesView)dgGameList.SelectedItem);
            }
            else
            {
                // multiple rows selected
                var rows = dgGameList.SelectedItems;
                foreach (DataGridGamesView r in rows)
                {
                    games.Add(r);
                }
            }

            // parse list of games to the method to be auto-scraped
            ScraperHandler.ScrapeGamesMultiple(games, ScrapeType.Selected);            
        }

        private void ReScrapeGamesMultiple_Click(object sender, RoutedEventArgs e)
        {
            // get number of selected rows
            int numOfRows = dgGameList.SelectedItems.Count;
            List<DataGridGamesView> games = new List<DataGridGamesView>();

            if (numOfRows == 0)
                return;
            else if (numOfRows == 1)
            {
                // single row selected
                games.Add((DataGridGamesView)dgGameList.SelectedItem);
            }
            else
            {
                // multiple rows selected
                var rows = dgGameList.SelectedItems;
                foreach (DataGridGamesView r in rows)
                {
                    games.Add(r);
                }
            }

            // parse list of games to the method to be auto-scraped
            ScraperHandler.ScrapeGamesMultiple(games, ScrapeType.SelectedRescrape);
        }

        private void ScrapeGames_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("ScrapeGames", ""));
            ScraperHandler.ScrapeGames(sysId, ScrapeType.System);
        }

        private void RescrapeGames_Click(object sender, RoutedEventArgs e)
        {
            // get systemId from menu name
            string menuName = (sender as MenuItem).Name;
            int sysId = Convert.ToInt32(menuName.Replace("RescrapeGames", ""));

            ScraperHandler.ScrapeGames(sysId, ScrapeType.RescrapeAll);
        }

        // Games grid filter text box event
        private void tbFilterDatagrid_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            int system = 0;
            // determine which radiobutton is checked
            if (btnShowAll.IsChecked == true)
            {
                system = 0;
            }
            if (btnFavorites.IsChecked == true)
            {
                system = -1;
            }
            if (btnLynx.IsChecked == true)
            {
                system = 3;
            }
            if (btnGg.IsChecked == true)
            {
                system = 5;
            }
            if (btnGba.IsChecked == true)
            {
                system = 2;
            }
            if (btnGb.IsChecked == true)
            {
                system = 1;
            }
            if (btnWswan.IsChecked == true)
            {
                system = 15;
            }
            if (btnNgp.IsChecked == true)
            {
                system = 6;
            }
            if (btnVb.IsChecked == true)
            {
                system = 14;
            }
            if (btnPcfx.IsChecked == true)
            {
                system = 8;
            }
            if (btnPce.IsChecked == true)
            {
                system = 7;
            }

            if (btnPsx.IsChecked == true)
            {
                system = 9;
            }
            if (btnSs.IsChecked == true)
            {
                system = 13;
            }

            if (btnMd.IsChecked == true)
            {
                system = 4;
            }
            if (btnSms.IsChecked == true)
            {
                system = 10;
            }
            if (btnSnes.IsChecked == true)
            {
                system = 12;
            }
            if (btnNes.IsChecked == true)
            {
                system = 11;
            }
            if (btnPcecd.IsChecked == true)
            {
                system = 18;
            }

            DbEF.GetGames(dgGameList, system, textbox.Text);
        }

        // Clear all filters button click
        public void btnClearFilters_Click(object sender, RoutedEventArgs e)
        {
            btnShowAll.IsChecked = true;
            tbFilterDatagrid.Clear();
        }

        // Games Datagrid selection changed
        private void dgGameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // choose context menu to show based on single or multiple selection
            var dg = sender as DataGrid;
            ContextMenu menuToUse;
            int numRowsSelected = dgGameList.SelectedItems.Count;
            var rs = dgGameList.SelectedItems;
            List<DataGridGamesView> rows = new List<DataGridGamesView>();

            if (numRowsSelected != 0)
            {
                foreach (DataGridGamesView r in rs)
                {
                    rows.Add(r);
                }
            }            

            if (numRowsSelected == 0)
            {
                // nothing select - refresh sidebar
                GamesLibraryVisualHandler.UpdateSidebar();
            }
            else if (numRowsSelected == 1)
            {
                // update right hand pane with game details
                GamesLibraryVisualHandler.UpdateSidebar(rows.First().ID);
                // set single context menu
                menuToUse = (ContextMenu)this.Resources["cmGamesListSingle"];
                dg.ContextMenu = menuToUse;
            }
            else
            {
                // multiple rows are selected - just take the first one to update sidebar
                GamesLibraryVisualHandler.UpdateSidebar(rows.First().ID);
                // set multiple context menu
                menuToUse = (ContextMenu)this.Resources["cmGamesListMultiple"];
                dg.ContextMenu = menuToUse;
            }
        }

        // generic TextBox_TextChanged handler
        public void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            var binding = ((TextBox)sender).GetBindingExpression(TextBox.TextProperty);
            binding.UpdateSource();

        }

        // generic Slider_ValueChanged handler
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var binding = ((Slider)sender).GetBindingExpression(Slider.ValueProperty);
            binding.UpdateSource();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // if check for updates on start is allowed
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"Data\Settings\MedLaunch.db"))
            {
                bool updateCheck = GlobalSettings.GetGlobals().checkUpdatesOnStart.Value;
                if (updateCheck == true)
                {
                    UpdateCheck(true);
                }

                // if import configs on start is enabled then import configs
                bool importCheck = GlobalSettings.GetGlobals().importConfigsOnStart.Value;
                if (importCheck == true && Directory.Exists(Paths.GetPaths().mednafenExe))
                {
                    ConfigImport ci = new ConfigImport();
                    ci.ImportAll(null);
                }
            }

            //System.Windows.Data.CollectionViewSource globalSettingsViewModelViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("globalSettingsViewModelViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // globalSettingsViewModelViewSource.Source = [generic data source]

            SettingsDirtyFlag = true; // now safe to save settings

            MiscVisualHandler.RefreshCoreVisibilities();
        }

        // Netplay Settings - netplay page
        private void btnNetplaySaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ConfigNetplaySettings.SaveNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
            //lblNpSettingsSave.Content = "***Netplay Settings Saved***";
        }

        private void btnNetplayCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            ConfigNetplaySettings.LoadNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
            //lblNpSettingsSave.Content = "***Netplay Settings Reverted***";
        }

        // Global Settings - Game page

        private void chkEnableNetplay_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableNetplay(chkEnableNetplay);
        }
        private void chkEnableNetplay_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableNetplay(chkEnableNetplay);
        }
        private void chkEnablePce_fast_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnablePce_fast(chkEnablePce_fast);
        }

        private void chkEnablePce_fast_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnablePce_fast(chkEnablePce_fast);
        }

        private void chkEnableSnes_faust_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableSnes_faust(chkEnableSnes_faust);
            // enable faust config if its not already
        }

        private void chkEnableSnes_faust_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableSnes_faust(chkEnableSnes_faust);
        }

        private void chkrememberSysWinPositions_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateRememberSysWinPositions(chkrememberSysWinPositions);
        }

        private void chkrememberSysWinPositions_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateRememberSysWinPositions(chkrememberSysWinPositions);
        }

        /*
        private void chkAllBaseSettings_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdatechkAllBaseSettings(chkAllBaseSettings);
            ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, 2000000000);
            btnConfigLynx.IsChecked = true;
            btnConfigBase.IsChecked = true;
            //btnConfigBase.IsChecked = false;
            //btnConfigBase.IsChecked = true;
        }

        private void chkAllBaseSettings_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdatechkAllBaseSettings(chkAllBaseSettings);
            ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, 2000000000);
            btnConfigLynx.IsChecked = true;
            btnConfigBase.IsChecked = true;
            //btnConfigBase.IsChecked = false;
            //btnConfigBase.IsChecked = true;
        }
        */

        private void chkMinToTaskbar_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateMinToTaskBar(chkMinToTaskbar);
        }

        private void chkMinToTaskbar_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateMinToTaskBar(chkMinToTaskbar);
        }

        private void chkHideSidebar_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateHideSidebar(chkHideSidebar);
        }

        private void chkHideSidebar_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateHideSidebar(chkHideSidebar);
        }

        private void chkshowGLCoop_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLCoop(chkshowGLCoop);
        }

        private void chkshowGLCoop_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLCoop(chkshowGLCoop);
        }

        private void chkshowGLDeveloper_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLDeveloper(chkshowGLDeveloper);
        }

        private void chkshowGLDeveloper_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLDeveloper(chkshowGLDeveloper);
        }

        private void chkshowGLESRB_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLESRB(chkshowGLESRB);
        }

        private void chkshowGLESRB_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLESRB(chkshowGLESRB);
        }

        private void chkshowGLPlayers_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLPlayers(chkshowGLPlayers);
        }

        private void chkshowGLPlayers_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLPlayers(chkshowGLPlayers);
        }

        private void chkshowGLPublisher_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLPublisher(chkshowGLPublisher);
        }

        private void chkshowGLPublisher_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLPublisher(chkshowGLPublisher);
        }

        private void chkshowGLYear_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLYear(chkshowGLYear);
        }

        private void chkshowGLYear_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateshowGLYear(chkshowGLYear);
        }


        private void chkPreferGenesis_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdatePreferGenesis(chkPreferGenesis);
        }

        private void chkPreferGenesis_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdatePreferGenesis(chkPreferGenesis);
        }

        private void chkAllowUpdateCheck_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateCheckUpdatesOnStart(chkAllowUpdateCheck);
        }

        private void chkAllowUpdateCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateCheckUpdatesOnStart(chkAllowUpdateCheck);
        }

        private void chkBackupMednafenConfig_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateBackupMednafenConfig(chkBackupMednafenConfig);
        }

        private void chkBackupMednafenConfig_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateBackupMednafenConfig(chkBackupMednafenConfig);
        }

        private void chkSaveSysConfigs_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateSaveSysConfigs(chkSaveSysConfigs);
        }

        private void chkSaveSysConfigs_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateSaveSysConfigs(chkSaveSysConfigs);
        }

        private void chkLoadConfigsOnStart_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateImportConfigsOnStart(chkLoadConfigsOnStart);
        }

        private void chkLoadConfigsOnStart_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateImportConfigsOnStart(chkLoadConfigsOnStart);
        }

        private void chkEnableConfigToolTips_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableConfigToolTips(chkEnableConfigToolTips);
        }

        private void chkEnableConfigToolTips_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableConfigToolTips(chkEnableConfigToolTips);
        }

        private void chkEnableClearCacheOnExit_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableClearCacheOnExit(chkEnableClearCacheOnExit);
        }

        private void chkEnableClearCacheOnExit_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateEnableClearCacheOnExit(chkEnableClearCacheOnExit);
        }




        private void chkAllowBanners_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowBanners(chkAllowBanners);
        }

        private void chkAllowBanners_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowBanners(chkAllowBanners);
        }

        private void chkAllowBoxart_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowBoxart(chkAllowBoxart);
        }

        private void chkAllowBoxart_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowBoxart(chkAllowBoxart);
        }

        private void chkAllowScreenshots_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowScreenshots(chkAllowScreenshots);
        }

        private void chkAllowScreenshots_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowScreenshots(chkAllowScreenshots);
        }

        private void chkAllowFanart_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowFanart(chkAllowFanart);
        }

        private void chkAllowFanart_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowFanart(chkAllowFanart);
        }

        private void chkAllowMedia_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowMedias(chkAllowMedia);
        }

        private void chkAllowMedia_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowMedias(chkAllowMedia);
        }

        private void chkAllowManuals_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowManuals(chkAllowManuals);
        }

        private void chkAllowManuals_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateAllowManuals(chkAllowManuals);
        }

        private void chkSecondaryScraperBackup_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateScraperBackup(chkSecondaryScraperBackup);
        }

        private void chkSecondaryScraperBackup_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalSettings.UpdateScraperBackup(chkSecondaryScraperBackup);
        }

        private void rbGDB_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings g = GlobalSettings.GetGlobals();
            g.primaryScraper = 1;
            GlobalSettings.SetGlobals(g);
        }

        private void rbMoby_Checked(object sender, RoutedEventArgs e)
        {
            GlobalSettings g = GlobalSettings.GetGlobals();
            g.primaryScraper = 2;
            GlobalSettings.SetGlobals(g);
        }


        // Mednafen BIOS Paths events
        private void btnMednafenBiosPaths_Click(object sender, RoutedEventArgs e)
        {
            // convert the button name
            string btnName = ((sender as Button).Name).Replace("tbBios", "");
            // textbox name
            string tbName = "";
            switch (btnName)
            {
                case "Gba":
                    tbName = "cfg_gba__bios";
                    break;
                case "PceGe":
                    tbName = "cfg_pce__gecdbioss";
                    break;
                case "PceCd":
                    tbName = "cfg_pce__cdbios";
                    break;
                case "PceFastCd":
                    tbName = "cfg_pce_fast__cdbios";
                    break;
                case "Pcfx":
                    tbName = "cfg_pcfx__bios";
                    break;
                case "MdCd":
                    tbName = "cfg_md__cdbios";
                    break;
                case "NesGg":
                    tbName = "cfg_nes__ggrom";
                    break;
                case "SsJp":
                    tbName = "cfg_ss__bios_jp";
                    break;
                case "SsNaEu":
                    tbName = "cfg_ss__bios_na_eu";
                    break;
                case "PsxEu":
                    tbName = "cfg_psx__bios_eu";
                    break;
                case "PsxJp":
                    tbName = "cfg_psx__bios_jp";
                    break;
                case "PsxNa":
                    tbName = "cfg_psx__bios_na";
                    break;

            }
            //string tbName = "cfg_filesys__path_" + btnName;
            // get textbox
            TextBox tb = (TextBox)(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()).FindName(tbName);

            OpenFileDialog filePath = new OpenFileDialog();
            filePath.Multiselect = false;
            filePath.Title = "Select " + btnName + " BIOS/Rom";
            filePath.ShowDialog();

            if (filePath.FileName.Length > 0)
            {
                tb.Text = filePath.FileName;
            }

        }

        // Mednafen Paths (cheats, saves etc..) events
        private void btnMednafenPaths_Click(object sender, RoutedEventArgs e)
        {
            // convert the button name
            string btnName = ((sender as Button).Name).ToLower().Replace("btnpath", "");
            // textbox name
            string tbName = "cfg_filesys__path_" + btnName;
            // get textbox
            TextBox tb = (TextBox)(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()).FindName(tbName);

            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select " + btnName + " Directory";
            path.ShowDialog();

            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tb.Text = strPath;
            }



        }

        // Path Page button clicks
        private void btnPathMednafen_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Mednafen Directory";
            path.ShowDialog();

            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathMednafen.Text = strPath;
            }

        }

        private void btnPathGb_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Gameboy ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathGb.Text = strPath;
            }
        }

        private void btnPathGba_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Gameboy Advance ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathGba.Text = strPath;
            }
        }

        private void btnPathGg_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select GameGear ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathGg.Text = strPath;
            }
        }

        private void btnPathLynx_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Atari Lynx ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathLynx.Text = strPath;
            }
        }

        private void btnPathMd_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Megadrive/Genesis ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathMd.Text = strPath;
            }

        }

        private void btnPathNes_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select NES ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathNes.Text = strPath;
            }
        }

        private void btnPathSnes_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select SNES ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathSnes.Text = strPath;
            }
        }

        private void btnPathNgp_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select NeoGeo Pocket Color ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathNgp.Text = strPath;
            }
        }

        private void btnPathPce_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select PC-Engine ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathPce.Text = strPath;
            }
        }

        private void btnPathPcfx_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select PC-FX ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathPcfx.Text = strPath;
            }
        }



        private void btnPathMs_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Sega Master System ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathSms.Text = strPath;
            }
        }

        private void btnPathPsx_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select PSX ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathPsx.Text = strPath;
            }
        }

        private void btnPathSs_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Sega Saturn ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathSs.Text = strPath;
            }
        }

        private void btnPathVb_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select Virtual Boy ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathVb.Text = strPath;
            }
        }

        private void btnPathWswan_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog path = new VistaFolderBrowserDialog();
            path.ShowNewFolderButton = true;
            path.Description = "Select WonderSwan ROM Directory";
            path.ShowDialog();
            if (path.SelectedPath != "")
            {
                string strPath = path.SelectedPath;
                tbPathWswan.Text = strPath;
            }
        }

        // Path page save & restore

        private void btnPathsSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            Paths.SavePathSettings(tbPathMednafen, tbPathGb, tbPathGba, tbPathGg, tbPathLynx, tbPathMd, tbPathNes, tbPathSnes, tbPathNgp, tbPathPce, tbPathPcfx, tbPathSms, tbPathVb, tbPathWswan); // tbPathPsx, tbPathSs);
            //lblPathsSettingsSave.Content = "***Path Settings Saved***";
        }

        private void btnPathsCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            Paths.LoadPathSettings(tbPathMednafen, tbPathGb, tbPathGba, tbPathGg, tbPathLynx, tbPathMd, tbPathNes, tbPathSnes, tbPathNgp, tbPathPce, tbPathPcfx, tbPathSms, tbPathVb, tbPathWswan); // tbPathPsx, tbPathSs);
            //lblPathsSettingsSave.Content = "***Path Settings Reverted***";
        }

        // games list context menu
        private void dgGameList_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            dgGameList.Items.Refresh();
            dgGameList.InvalidateVisual();
            FrameworkElement fe = e.Source as FrameworkElement;
            ContextMenu cm = fe.ContextMenu;

            ContextMenu c = (ContextMenu)this.FindName("dgContext");

            // get selected row data
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            if (drv == null)
            {
                c = (ContextMenu)this.FindName("dgContext");
                //c.Visibility = Visibility.Collapsed;
                //c.IsOpen = false;
                return;
            }
            // c.Visibility = Visibility.Visible;


            //MessageBox.Show(drv.ID.ToString());
            string romName = drv.Game;
            int romId = drv.ID;

            // Replace Menu Items
            foreach (MenuItem mi in cm.Items)
            {
                // check whether selection is empty - if so dont display the menu item

                // play game menu item
                if ((String)mi.Header == "Play Game")
                {
                    mi.Header = "Play Game"; // + romName;
                }

                // Favorites toggle
                if ((String)mi.Header == "Favorites")
                {
                    // check the favorite status
                    if (GameScanner.GetFavoriteStatus(romId) == 1)
                        mi.Header = "Add/Remove From Favorites";
                    else
                        mi.Header = "Add/Remove From Favorites";
                }

                // remove game
                if ((String)mi.Header == "Delete From Games Library")
                {
                    mi.Header = "Delete From Games Library"; // + romName;
                }
            }




            //fe.ContextMenu = CMenu.BuildGamesMenu(dgGameList);
        }

        private void MenuItemFavorite_Click(object sender, RoutedEventArgs e)
        {
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            int romId = drv.ID;
            GameScanner.FavoriteToggle(romId);
            // refresh library view
            GameListBuilder.UpdateFlag();
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void MenuItemFavorites_Click(object sender, RoutedEventArgs e)
        {
            int numRows = dgGameList.SelectedItems.Count;

            if (numRows == 0)
                return;
            else if (numRows == 1)
            {
                DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
                int romId = drv.ID;
                GameScanner.FavoriteToggle(romId);
            }
            else
            {
                var rs = dgGameList.SelectedItems;
                List<DataGridGamesView> games = new List<DataGridGamesView>();
                foreach (DataGridGamesView row in rs)
                {
                    games.Add(row);
                }

                foreach (var game in games)
                {
                    GameScanner.FavoriteToggle(game.ID);
                }
            }

            // refresh library view
            GameListBuilder.UpdateFlag();
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void DeleteRom_Click(object sender, RoutedEventArgs e)
        {
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            int romId = drv.ID;
            Game game = Game.GetGame(romId);
            // delete from library
            Game.DeleteGame(game);
            

            // refresh library view
            GameListBuilder.UpdateFlag();
           GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void DeleteRoms_Click(object sender, RoutedEventArgs e)
        {
            int numRows = dgGameList.SelectedItems.Count;

            if (numRows == 0)
                return;
            else if (numRows == 1)
            {
                DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
                int romId = drv.ID;
                Game game = Game.GetGame(romId);
                // delete from library
                Game.DeleteGame(game);
            }
            else
            {
                var rs = dgGameList.SelectedItems;
                List<DataGridGamesView> rows = new List<DataGridGamesView>();
                foreach (DataGridGamesView row in rs)
                {
                    rows.Add(row);
                }

                List<Game> games = new List<Game>();

                foreach (DataGridGamesView row in rows)
                {
                    int id = row.ID;
                    Game game = Game.GetGame(id);
                    games.Add(game);
                }

                Game.DeleteGames(games);
            }

            // refresh library view
            GameListBuilder.UpdateFlag();
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void CopyLaunchStringToClipboard_Click(object sender, RoutedEventArgs e)
        {
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            int romId = drv.ID;
            GameLauncher.CopyLaunchStringToClipboard(romId);
        }

        private async void LaunchRomShowConfig_Click(object sender, RoutedEventArgs e)
        {
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            if (drv == null)
                return;
            int romId = drv.ID;

            bool b = Versions.MednafenVersionCheck();

            if (b == false)
            {
                return;
            }

            // create new GameLauncher instance
            GameLauncher gl = new GameLauncher(romId);

            // get base config params
            string configCmdString = gl.GetCommandLineArguments();

            // set to launchstring variable
            LaunchString = configCmdString;

            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            await mw.ShowChildWindowAsync(new LaunchStringWindow()
            {
                IsModal = true,
                AllowMove = false,
                Title = "View / Modify Launch String",
                CloseOnOverlay = false,
                ShowCloseButton = false
            }, RootGrid);
        }

        private void LaunchRom_Click(object sender, RoutedEventArgs e)
        {
            int numRowsCount = dgGameList.SelectedItems.Count;
            if (numRowsCount != 1)
                return;

            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            if (drv == null)
                return;
            int romId = drv.ID;

            bool b = Versions.MednafenVersionCheck();

            if (b == false)
            {
                return;
            }

            // create new GameLauncher instance
            GameLauncher gl = new GameLauncher(romId);
            LaunchString = gl.GetCommandLineArguments();
            LaunchRomHandler(LaunchString, false);
        }

        public async void LaunchRomHandler(string cmdlineargs, bool bypassSystemConfigs)
        {
            DataGridGamesView drv = (DataGridGamesView)dgGameList.SelectedItem;
            if (drv == null)
                return;
            int romId = drv.ID;
            int systemId = Game.GetGame(romId).systemId;

            bool b = Versions.MednafenVersionCheck();

            if (b == false)
            {
                return;
            }

            // create new GameLauncher instance
            GameLauncher gl = new GameLauncher(romId);


            // popup launch dialog
            var mySettings = new MetroDialogSettings()
            {
                //NegativeButtonText = "Cancel Scanning",
                AnimateShow = false,
                AnimateHide = false
            };

            var controller = await this.ShowProgressAsync("Launching " + gl.SystemName + " Game", "Starting: " + gl.RomName, settings: mySettings);


            controller.SetIndeterminate();

            await Task.Delay(100);

            controller.SetCancelable(false);

            string[] returnStr = gl.PathChecks();

            if (returnStr != null && returnStr.Length > 0)
            {
                string o = "";
                foreach (string s in returnStr)
                {
                    o += s + "\n";
                }
                controller.SetMessage(o + "\n...Cancelling Operation...");
                await Task.Delay(100);
                await controller.CloseAsync();
            }
            else
            {
                string status = "...Building config...\n";
                controller.SetMessage(status);
                await Task.Delay(50);

                string cfgName;
                if (gl.ConfigId == 2000000000)
                    cfgName = "Base Configuration";
                else
                    cfgName = gl.SystemName + " Configuration";

                status += "Using " + cfgName + "\n";
                controller.SetMessage(status);
                await Task.Delay(50);

                string netplayEnabled;
                if (gl.Global.enableNetplay == true)
                    netplayEnabled = "Netplay Enabled: Yes\nHost: " + gl.Server.ConfigServerDesc;
                else
                    netplayEnabled = "Netplay Enabled: No";
                status += netplayEnabled + "\n";
                controller.SetMessage(status);
                await Task.Delay(50);

                    string launchGame = "...Launching Game...";
                    status += launchGame + "\n";
                    controller.SetMessage(status);
                    await Task.Delay(50);

                    // check whether minimise to taskbar option is checked
                    bool taskbar = this.ShowInTaskbar;
                    if (GlobalSettings.Min2TaskBar() == true)
                    {
                        this.ShowInTaskbar = true;
                        this.WindowState = WindowState.Minimized;
                    }


                    // launch game
                    await Task.Run(() =>
                    {
                        // update lastplayed time
                        Game.SetStartedPlaying(gl.GameId);

                        // rename system configs if neccesary (this can be removed if/when Ryphecha implements a custom config cmdline option)
                        if (bypassSystemConfigs == true)
                        {
                            GameLauncher.SystemConfigsOff();
                        }

                        // launch game                        
                        gl.RunGame(cmdlineargs, systemId);

                        // name back system configs if neccesary (this can be removed if/when Ryphecha implements a custom config cmdline option)
                        if (bypassSystemConfigs == true)
                        {
                            GameLauncher.SystemConfigsOn();
                        }


                    });


                    if (GlobalSettings.Min2TaskBar() == true)
                    {
                        this.ShowInTaskbar = taskbar;
                        this.WindowState = WindowState.Normal;
                    }



                    await this.Dispatcher.Invoke(async () =>
                    {
                        controller.SetTitle("Cleaning Up");
                        controller.SetMessage("Please Wait....");
                        await Task.Delay(100);

                        // update lastfinished time
                        Game.SetFinishedPlaying(gl.GameId);

                        // update gameslibrary data as change has been made
                        GamesLibData.ForceUpdate();

                        // refresh library view
                        GamesLibraryVisualHandler.RefreshGamesLibrary();
                    });
                }                

                await controller.CloseAsync();

            GameListBuilder.UpdateFlag();
            GamesLibraryVisualHandler.UpdateSidebar(gl.GameId);


            //controller.SetMessage(totalFiles + " files found across all ROM directories");





            /*
            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("The operation was cancelled!", romsInserted +  " ROMS have been added \n" + romsUpdated + " ROMS have been updated \n" + romsSkipped + " ROMS have been skipped");
            }
            else
            {
                await this.ShowMessageAsync("Operation completed", romsInserted + " ROMS have been added \n" + romsUpdated + " ROMS have been updated \n" + romsSkipped + " ROMS have been skipped");
            }

            //Update list
            // ensure 'show all' filter is checked on startup
            btnFavorites.IsChecked = true;
            btnShowAll.IsChecked = true;

            */
            /*
            MessageBoxResult result = MessageBox.Show("RomId: " + gl.GameId.ToString());
            MessageBoxResult result1 = MessageBox.Show("SystemId: " + gl.SystemId.ToString());
            MessageBoxResult result2 = MessageBox.Show("RomName: " + gl.RomName.ToString());
            MessageBoxResult result3 = MessageBox.Show("RomFolder: " + gl.RomFolder.ToString());
            MessageBoxResult result4 = MessageBox.Show("RomPath: " + gl.RomPath.ToString());
            MessageBoxResult result5 = MessageBox.Show("MednafenPath: " + gl.MednafenFolder.ToString());
            */




        }


        // servers


       

        private void btnServerSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ConfigServerSettings.SaveCustomServerSettings(tbServerDesc, tbHostname, slServerPort, tbPassword, tbGameKey);
        }

        // controls tab

        // generic controls selections buttons
        private void btnControl_Checked(object sender, RoutedEventArgs e)
        {
            //ControlsVisualHandler.ButtonClick();

            RadioButton rb = sender as RadioButton;
            string rbName = rb.Name.Replace("btnControl", "").ToLower();

            // get all the filter buttons
            List<RadioButton> _filterButtons = UIHandler.GetLogicalChildCollection<RadioButton>(wpControlLeftPane);
            RadioButton[] arr = (from a in _filterButtons
                                 where a.Name != "tccControls"
                                 select a).ToArray();

            // hide all
            for (int i = 0; i < arr.Length; i++)
            {
                TransitioningContentControl t = (TransitioningContentControl)this.FindName("tccControlsDyn" + (i + 1).ToString());
                t.Content = null;
            }


            switch (rbName)
            {
                case "nes":
                    tccControls.Content = new NesCtrl();
                    break;
                case "gb":
                    tccControls.Content = new GbCtrl();
                    break;
                case "gba":
                    tccControls.Content = new GbaCtrl();
                    break;
                case "gg":
                    tccControls.Content = new GgCtrl();
                    break;
                case "lynx":
                    tccControls.Content = new LynxCtrl();
                    break;
                case "ngp":
                    tccControls.Content = new NgpCtrl();
                    break;
                case "md":
                    tccControls.Content = new MdCtrl();
                    break;
                case "snes":
                    tccControls.Content = new SnesCtrl();
                    break;
                case "snes_faust":
                    tccControls.Content = new Snes_FaustCtrl();
                    break;
                case "sms":
                    tccControls.Content = new SmsCtrl();
                    break;
                case "pce":
                    tccControls.Content = new PceCtrl();
                    break;
                case "pce_fast":
                    tccControls.Content = new Pce_FastCtrl();
                    break;
                case "vb":
                    tccControls.Content = new VbCtrl();
                    break;
                case "wswan":
                    tccControls.Content = new WswanCtrl();
                    break;
                case "pcfx":
                    tccControls.Content = new PcfxCtrl();
                    break;
                case "ss":
                    tccControls.Content = new SsCtrl();
                    break;
                case "psx":
                    tccControls.Content = new PsxCtrl();
                    break;
                default:
                    // show all usercontrols

                    // null the static one
                    tccControls.Content = null;

                    
                    // loop through and display each one
                    for (int i = 0; i < arr.Length; i++)
                    {
                        TransitioningContentControl t = (TransitioningContentControl)this.FindName("tccControlsDyn" + (i + 1).ToString());
                        switch (i + 1)
                        {
                            case 1:         // nes
                                t.Content = new NesCtrl();
                                break;
                            case 2:         // snes
                                t.Content = new SnesCtrl();
                                break;
                            case 3:         // snesfaust
                                t.Content = new Snes_FaustCtrl();
                                break;
                            case 4:         // sms
                                t.Content = new SmsCtrl();
                                break;
                            case 5:         // md
                                t.Content = new MdCtrl();
                                break;
                            case 6:         // pce
                                t.Content = new PceCtrl();
                                break;
                            case 7:         // pcefast
                                t.Content = new Pce_FastCtrl();
                                break;
                            case 8:         // vb
                                t.Content = new VbCtrl();
                                break;
                            case 9:         // npg
                                t.Content = new NgpCtrl();
                                break;
                            case 10:         // wswan
                                t.Content = new WswanCtrl();
                                break;
                            case 11:         // gb
                                t.Content = new GbCtrl();
                                break;
                            case 12:         // gba
                                t.Content = new GbaCtrl();
                                break;
                            case 13:         // gg
                                t.Content = new GgCtrl();
                                break;
                            case 14:         // lynx
                                t.Content = new LynxCtrl();
                                break;
                            case 15:         // ss
                                t.Content = new SsCtrl();
                                break;
                            case 16:         // psx
                                t.Content = new PsxCtrl();
                                break;
                            case 17:         // pcfx
                                t.Content = new PcfxCtrl();
                                break;
                                
                        }
                    }

                    break;
            }
        }

        private void btnControl_UnChecked(object sender, RoutedEventArgs e)
        {

        }

        private void Controller_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // get the combobox
            ComboBox cb = sender as ComboBox;
            string cbName = cb.Name;
            string selected = (e.AddedItems[0] as ComboBoxItem).Content.ToString();

            // now parse based on system
            string sysStr = cbName.Replace("cmb", "");

            // nes standard gamepad
            if (sysStr == "Nes1")
            {

            }
        }

        private async void btnControlsConfigure_Click(object sender, RoutedEventArgs e)
        {
            // get button name
            Button button = (Button)sender;
            string name = button.Name;

            // remove beginning and end
            name = name.Replace("btn", "").Replace("Configure", "");

            // get the relevant combox
            ComboBox cb = (ComboBox)this.FindName("cmb" + name);

            // get the virtual port number
            //ComboBoxItem typeItem = (ComboBoxItem)cb.SelectedItem;
            string selectedString = cb.SelectionBoxItem.ToString();
            int portNum = Convert.ToInt32(selectedString.Replace("Virtual Port ", ""));

            // Get device definition for this controller
            DeviceDefinition dev = Nes.GamePad(portNum);
            ControllerDefinition = dev;

            // launch controller configuration window
            await this.ShowChildWindowAsync(new ConfigureController()
            {
                IsModal = true,
                AllowMove = false,
                Title = "Controller Configuration",
                CloseOnOverlay = false,
                ShowCloseButton = false
            }, RootGrid);
        }


        // Config Tab               

        // Generic Config Selection Buttons CHECKED
        private void btnConfig_Checked(object sender, RoutedEventArgs e)
        {
            // get selected ConfigId
            var rb = sender as RadioButton;
            int systemIdSelected = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
            // Save all control values to database
            ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, systemIdSelected);
            ConfigsVisualHandler.HideControls(ConfigWrapPanel, systemIdSelected);
            ConfigsVisualHandler.ButtonClick();

            // populate the psx combo fields
            string hexCode = cfg_psx__input__analog_mode_ct__compare.Text;
            CalculateComboChecks(hexCode);

        }

        // Generic Config Selection Buttons UNCHECKED
        private void btnConfig_Unchecked(object sender, RoutedEventArgs e)
        {
            // get selected ConfigId
            var rb = sender as RadioButton;
            int systemIdSelected = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
            // Save all control values to database
            ConfigBaseSettings.SaveControlValues(ConfigWrapPanel, systemIdSelected);
        }

        // set config entry to defaults
        private void MenuItemDefault_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            RadioButton rb = null;

            if (mi != null)
            {
                rb = ((ContextMenu)mi.Parent).PlacementTarget as RadioButton;
                //MessageBoxResult result4 = MessageBox.Show("RadioButton Found...Name: " + rb.Name);

                // ensure button is selected
                rb.IsChecked = true;


                ConfigBaseSettings.ResetToDefault(rb.Name);
                ConfigBaseSettings.SetButtonState(rb);

                // refresh UI
                int systemIdSelected = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
                ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, systemIdSelected);


                lblConfigStatus.Content = "***Defaults Loaded***";
            }
        }

        // Enable system specific config entry
        private void MenuItemConfigEnable_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            RadioButton rb = null;

            if (mi != null)
            {
                rb = ((ContextMenu)mi.Parent).PlacementTarget as RadioButton;

                // ensure button is selected
                rb.IsChecked = true;

                //MessageBoxResult result4 = MessageBox.Show("RadioButton Found...Name: " + rb.Name);
                ConfigBaseSettings.EnableConfigToggle(rb.Name);
                ConfigBaseSettings.SetButtonState(rb);

                // refresh UI
                int systemIdSelected = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
                ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, systemIdSelected);

                //lblConfigStatus.Content = "***Config Enabled***";
            }
        }


        private void btnConfigSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // get config ID
            int configId = 2000000000;
            foreach (UIElement element in ConfigSelectorWrapPanel.Children)
            {
                if (element is RadioButton)
                {
                    // Is Radiobutton selected?
                    if ((element as RadioButton).IsChecked == true)
                    {
                        string rbName = (element as RadioButton).Name;
                        configId = ConfigBaseSettings.GetConfigIdFromButtonName(rbName);
                    }
                }
            }
            // save config changes
            ConfigBaseSettings.SaveControlValues(ConfigWrapPanel, configId);
            lblConfigStatus.Content = "***Config Saved***";
        }

        private void btnConfigCancelChanges_Click(object sender, RoutedEventArgs e)
        {
            // get config ID
            int configId = 2000000000;
            foreach (UIElement element in ConfigSelectorWrapPanel.Children)
            {
                if (element is RadioButton)
                {
                    // Is Radiobutton selected?
                    if ((element as RadioButton).IsChecked == true)
                    {
                        string rbName = (element as RadioButton).Name;
                        configId = ConfigBaseSettings.GetConfigIdFromButtonName(rbName);
                    }
                }
            }
            // re-load config changes
            ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, configId);
        }


        // settings tab events
        private void btnSettings_Checked(object sender, RoutedEventArgs e)
        {
            SettingsVisualHandler.ButtonClick();
        }

        private async void btnSettingsSaveAllChanges_Click(object sender, RoutedEventArgs e)
        {
            //SettingsHandler sh = new SettingsHandler();
            //sh.SaveAllSettings();
            
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scanning",
                AnimateShow = false,
                AnimateHide = true
            };

            var controller = await this.ShowProgressAsync("Please wait...", "Saving Settings\n(This may take a few seconds depending on your system)", settings: mySettings);
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            await Task.Delay(200);


            this.Dispatcher.Invoke(() =>
            {
                Paths.SavePathSettings(tbPathMednafen, tbPathGb, tbPathGba, tbPathGg, tbPathLynx, tbPathMd, tbPathNes, tbPathSnes, tbPathNgp, tbPathPce, tbPathPcfx, tbPathSms, tbPathVb, tbPathWswan);
                ConfigNetplaySettings.SaveNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
                ConfigServerSettings.SaveCustomServerSettings(tbServerDesc, tbHostname, slServerPort, tbPassword, tbGameKey);
                //ConfigBaseSettings.SaveMednafenPathValues(spMedPathSettings);
                ConfigBaseSettings.SaveMednafenPaths();
                //ConfigBaseSettings.SaveBiosPathValues(spSysBiosSettings);
                ConfigBaseSettings.SaveBiosPaths();

                // global settings
                GlobalSettings gs = GlobalSettings.GetGlobals();
                gs.maxFanarts = slFanrtsPerHost.Value;
                gs.maxScreenshots = slScreenshotsPerHost.Value;
                gs.imageToolTipPercentage = Convert.ToDouble(comboImageTooltipSize.SelectedValue);
                GlobalSettings.SetGlobals(gs);
            });

            await controller.CloseAsync();


        }

        private void btnSettingsCancelAllChanges_Click(object sender, RoutedEventArgs e)
        {
            //SettingsHandler sh = new SettingsHandler();
            //sh.LoadAllSettings();

            Paths.LoadPathSettings(tbPathMednafen, tbPathGb, tbPathGba, tbPathGg, tbPathLynx, tbPathMd, tbPathNes, tbPathSnes, tbPathNgp, tbPathPce, tbPathPcfx, tbPathSms, tbPathVb, tbPathWswan);
            ConfigNetplaySettings.LoadNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
            //ConfigServerSettings.PopulateCustomServer(tbServerDesc, tbHostname, slServerPort, tbPassword, tbGameKey);
            ConfigBaseSettings.LoadMednafenPathValues(spMedPathSettings);
            ConfigBaseSettings.LoadBiosPathValues(spSysBiosSettings);

        }

        public class OneReturnsTrueConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return (int)value == 1;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        private void DataGrid_PreviewMouseRightButtonDownBlock(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Preview MouseRightButtonDown");

            e.Handled = true;
        }


        private void DataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            /* If any right-clicks happen where there is no datagrid row clicked on - dont open the context menu */

            DependencyObject DepObject = (DependencyObject)e.OriginalSource;
            /*
            while ((DepObject != null) && !(DepObject is DataGridColumnHeader))
            {
                DepObject = VisualTreeHelper.GetParent(DepObject);
            }

            if (DepObject == null)
            {                
                return;
            }


            if (DepObject is DataGridColumnHeader)
            {
                // prevent right clicks on header values
                e.Handled = true;
            }
            */

            DataGrid dg = sender as DataGrid;
            Point pt = e.GetPosition(dg);
            DataGridCell dgc = null;
            VisualTreeHelper.HitTest(dg, null, new HitTestResultCallback((result) =>
            {
                // Find the ancestor element form the hittested element
                // e.g., find the DataGridCell if we hittest on the inner TextBlock
                DataGridCell cell = FindVisualParent<DataGridCell>(result.VisualHit);
                if (cell != null)
                {
                    dgc = cell;
                    return HitTestResultBehavior.Stop;
                }
                else
                    return HitTestResultBehavior.Continue;
            }), new PointHitTestParameters(pt));

            if (dgc == null)
            {
                //MessageBox.Show("NULL!");
                e.Handled = true;
            }

        }

        public static Visibility IsDebug
        {
#if DEBUG
            get { return Visibility.Visible; }
#else
        get { return Visibility.Collapsed; }
#endif
        }

        private void btnSavePlatformGamesToDisk_Click(object sender, RoutedEventArgs e)
        {
            string linkTimeLocal = (Assembly.GetExecutingAssembly().GetLinkerTime()).ToString("yyyy-MM-dd HH:mm:ss");
            App app = ((App)Application.Current);
            var platformgames = app.ScrapedData.MasterPlatformList;

            string json = JsonConvert.SerializeObject(platformgames.ToArray());
            System.IO.File.WriteAllText(@"Data\Settings\thegamesdbplatformgames_" + linkTimeLocal.Replace(" ", "").Replace(":", "").Replace("-", "") + ".json", json);

        }

        // unlink selected game
        private void btnScrapingUnlinkGame_Click(object sender, RoutedEventArgs e)
        {
            ScraperHandler.UnlinkGameData(dgGameList);
            GamesLibraryVisualHandler.RefreshGamesLibrary();
        }

        private void btnTestGameSearch_Click(object sender, RoutedEventArgs e)
        {
            /*
            GameScraper gs = new GameScraper();
            Game game = Game.GetGame(20);

            List<Game> games = Game.GetGames(7);

            foreach (Game g in games)
            {
                List<ScraperMaster> result = gs.SearchGameLocal(g.gameName, g.systemId, g.gameId).ToList();
                string glist = g.gameName + "\n-------------------------\n\n";
                foreach (ScraperMaster bg in result)
                {
                    glist += bg.TGDBData.GamesDBTitle + "\n";
                }
                MessageBox.Show(glist);
            }
            */

            //List<GDBPlatformGame> result = gs.SearchGameLocal(game.gameName, game.systemId, game.gameId).ToList();
        }

        private void btnScrapingPickGames_Click(object sender, RoutedEventArgs e)
        {
            ScraperMainSearch.PickGames(dgGameList);
        }

        private void btnScrapingPickGame_Click(object sender, RoutedEventArgs e)
        {
            ScraperMainSearch.PickGame(dgGameList);
        }

        private void btnBrowseDataFolder_Click(object sender, RoutedEventArgs e)
        {
            var r = (DataGridGamesView)dgGameList.SelectedItem;
            // get the gamesdbid

            int gdbId = 0;

            Game ga = Game.GetGame(Convert.ToInt32(r.ID));
            if (ga.gdbId == null || ga.gdbId == 0)
                return;
            else
            {
                gdbId = ga.gdbId.Value;
            }
            
            if (gdbId > 0)
            {
                // open the folder in windows explorer
                string dirPath = System.AppDomain.CurrentDomain.BaseDirectory + @"\Data\Games\" + gdbId;
                // check folder exists
                if (Directory.Exists(dirPath))
                {
                    // open the folder
                    Process.Start(dirPath);
                }
            }

        }

        private async void btnScrapingReScrape_Click(object sender, RoutedEventArgs e)
        {
            var r = (DataGridGamesView)dgGameList.SelectedItem;
            // get the gamesdbid
            if (Game.GetGame(Convert.ToInt32(r.ID)).gdbId == null)
                return;

            int gdbId = Game.GetGame(Convert.ToInt32(r.ID)).gdbId.Value;
            // re-scrape the game
            if (gdbId > 0)
            {
                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                var mySettings = new MetroDialogSettings()
                {
                    NegativeButtonText = "Cancel Scraping",
                    AnimateShow = false,
                    AnimateHide = false
                };
                var controller = await mw.ShowProgressAsync("Scraping Data", "Initialising...", true, settings: mySettings);

                ScraperHandler sh = new ScraperHandler(gdbId, r.ID);
                await Task.Delay(100);
                await Task.Run(() =>
                {
                    if (controller.IsCanceled)
                    {
                        controller.CloseAsync();
                        return;
                    }
                    sh.ScrapeGame(controller);
                });

                await controller.CloseAsync();

                if (controller.IsCanceled)
                {
                    await mw.ShowMessageAsync("MedLaunch Scraper", "Scraping Cancelled");
                    //GamesLibraryVisualHandler.RefreshGamesLibrary();
                }
                else
                {
                    await mw.ShowMessageAsync("MedLaunch Scraper", "Scraping Completed");
                }

                var ro = (DataGridGamesView)dgGameList.SelectedItem;
                dgGameList.SelectedItem = null;
                dgGameList.SelectedItem = ro;

            }
        }

        private void btnOpenManual_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (sender as Button);
            string path = btn.ToolTip.ToString();
            Process.Start(path);
        }

        // save the layout of all the games library expander states
        private void GamesLibraryExpanderSaveLayout(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("expander triggered");
            GamesLibraryVisualHandler.SaveExpanderStates();
        }

        private void MW_Closing(object sender, CancelEventArgs e)
        {

            // save games library expander states
            GamesLibraryVisualHandler.SaveExpanderStates();

            // clear the rom cache folder if this option has been enabled
            GlobalSettings gs = GlobalSettings.GetGlobals();
            if (gs.enableClearCacheOnExit == true)
            {
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\Cache"))
                {
                    FileAndFolder.ClearFolder(AppDomain.CurrentDomain.BaseDirectory + "Data\\Cache");
                }
                
            }

        }

        private void TabItem_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void tiSettings_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private async void UpdateCheck(bool isStartup)
        {
            Release newRelease = new Release();

            // get current medlaunch version
            string currVersion = Versions.ReturnApplicationVersion();
            //string currVersion = "0.2.8.0";

            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Update Check",
                AnimateShow = true,
                AnimateHide = true

            };

            var controller = await this.ShowProgressAsync("Checking for Updates", "Connecting to Github", settings: mySettings);
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            await Task.Delay(400);

            string output;

            // attempt to download the LatestVersion text file from github
            string contents;

            using (var wc = new CustomWebClient())
            {
                wc.Proxy = null;
                wc.Timeout = 2000;
                string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)";
                wc.Headers.Add("user-agent", userAgent);
                try
                {
                    //contents = wc.DownloadString("https://raw.githubusercontent.com/Asnivor/MedLaunch/master/MedLaunch/LatestVersion.txt");
                    var c = wc.DownloadString("https://api.github.com/repos/Asnivor/MedLaunch/releases/latest");
                    contents = c;
                }
                catch (Exception ex)
                {
                    controller.SetMessage("The request timed out - please try again");
                    await Task.Delay(2000);
                    await controller.CloseAsync();
                    wc.Dispose();
                    return;
                }
                finally
                {
                    wc.Dispose();
                }

            }


            controller.SetMessage("Determining latest version...");
            // dynamically parse the github json latest release info
            dynamic d = JObject.Parse(contents);

            // check whether the version is greater than the one we have installed
            string latestVersion = d.tag_name;
            // compare versions and determine whether an upgrade is needed
            string[] CurrVersionArr = currVersion.Split('.');
            string[] newVersionArr = latestVersion.Split('.');
            bool upgradeNeeded = false;

            for (int v = 0; v < 4; v++)
            {
                int currV = Convert.ToInt32(CurrVersionArr[v]);
                int currN = Convert.ToInt32(newVersionArr[v]);

                if (currV > currN)
                {
                    // current version is NEWER than new version - upgrade not needed - break
                    break;
                }
                if (currV == currN)
                {
                    // versions are the same - continue checking
                    continue;
                }
                if (currV < currN)
                {
                    // new version is greater than old for this octet - upgrade needed
                    upgradeNeeded = true;
                    break;
                }

            }

            if (upgradeNeeded == true)
            {
                output = "A New MedLaunch Release is Now Available";
                UpdatedHeader.Header = "**UPDATE AVAILABLE**";
                //await Task.Delay(1000);
                controller.SetMessage("Downloading release information");
                await Task.Delay(500);
                string releaseInfo;

                // get release info from JSON
                releaseInfo = d.body;

                
            }
            else
            {
                output = "Your Version of MedLaunch is up to date";
                UpdatedHeader.Header = "Updates";
            }
            controller.SetMessage(output);
            if (isStartup == false)
                await Task.Delay(1000);

            await controller.CloseAsync();

            // update the UI if needed
            if (upgradeNeeded == true)
            {
                lbl1.Visibility = Visibility.Visible;
                lbl2.Visibility = Visibility.Visible;
                lbl3.Visibility = Visibility.Visible;
                lbl4.Visibility = Visibility.Visible;
                lbl5.Visibility = Visibility.Visible;

                // new version
                lblVersion.Visibility = Visibility.Visible;
                lblVersion.Content = d.tag_name;

                // release date
                lblDate.Visibility = Visibility.Visible;    // release date
                string sDate = d.published_at;
                string[] sD = sDate.Split('T');
                lblDate.Content = sD[0];

                // release notes
                tbNotes.Visibility = Visibility.Visible;
                tbNotes.Text = d.body;

                // download URL
                lblDownloadUrl.Visibility = Visibility.Visible;
                lblDownloadUrl.Content = d.assets[0].browser_download_url;

                btnUpdate.Visibility = Visibility.Visible;
                lblNoUpdate.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblVersion.Visibility = Visibility.Collapsed;
                lblDate.Visibility = Visibility.Collapsed;
                tbNotes.Visibility = Visibility.Collapsed;
                lblDownloadUrl.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                lbl1.Visibility = Visibility.Collapsed;
                lbl2.Visibility = Visibility.Collapsed;
                lbl3.Visibility = Visibility.Collapsed;
                lbl4.Visibility = Visibility.Collapsed;
                lbl5.Visibility = Visibility.Collapsed;
                lblNoUpdate.Visibility = Visibility.Visible;
            }
        }

        private void btnCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheck(false);
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            /* start download and autoupdate */


            string downloadsFolder = System.AppDomain.CurrentDomain.BaseDirectory + @"Data\Updates";
            System.IO.Directory.CreateDirectory(downloadsFolder);

            // get the new version
            string v = lblVersion.Content.ToString();
            string[] vArr = v.Split('.');

            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Download",
                AnimateShow = true,
                AnimateHide = true

            };

            var controller = await this.ShowProgressAsync("MedLaunch Update", "Downloading MedLaunch v" + v, settings: mySettings);
            controller.SetCancelable(false);
            controller.SetIndeterminate();

            await Task.Delay(400);

            string output;

            // download url
            string url = lblDownloadUrl.Content.ToString();
            // get just the filename
            string[] fArr = url.Split('/');
            string fName = fArr[fArr.Length - 1];

            // try the download

            using (var wc = new CustomWebClient())
            {
                wc.Proxy = null;
                wc.Timeout = 2000;
                try
                {
                    wc.DownloadFile(url, downloadsFolder + "\\" + fName);
                }
                catch (Exception ex)
                {
                    controller.SetMessage("The request timed out - please try again");
                    await Task.Delay(2000);
                    await controller.CloseAsync();
                    wc.Dispose();
                    return;
                }
                finally
                {
                    wc.Dispose();
                }
            }


            // now run updater app to extract MedLaunch over the existing directory
            // build command line args
            string processArg = "/P:" + Process.GetCurrentProcess().Id.ToString();
            string upgradeArg = "/U:" + fName; // "MedLaunch_v" + vArr[0] + "_" + vArr[1] + "_" + vArr[2] + "_" + vArr[3] + ".zip";
            string args = processArg + " " + upgradeArg;
            // call the external updater app and close this one
            // call the updater app and close this one
            Process.Start("lib\\Updater.exe", args);
            Thread.Sleep(2000);
            Environment.Exit(0);


            await controller.CloseAsync();


        }

        private class CustomWebClient : System.Net.WebClient
        {
            public int Timeout { get; set; }

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest lWebRequest = base.GetWebRequest(uri);
                lWebRequest.Timeout = Timeout;
                ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
                ((HttpWebRequest)lWebRequest).KeepAlive = false;
                return lWebRequest;
            }
        }

        private async void btnmobyPlatformList_Click(object sender, RoutedEventArgs e)
        {

            // get the main window
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            // start progress dialog controller
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scraping",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await mw.ShowProgressAsync("Scraping MobyGames (Basic) Games List", "Initialising...", true, settings: mySettings);
            controller.SetCancelable(true);
            await Task.Delay(100);

            await Task.Run(() =>
            {
                MobyScraper.ScrapeBasicGamesList(controller);
            });

            //MobyGames.ScrapeAllPlatformGames();
            // App app = ((App)Application.Current);

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await mw.ShowMessageAsync("Moby Master Games List Download", "Scraping Cancelled");
            }
            else
            {
                await mw.ShowMessageAsync("Moby Master Games List Download", "Scraping Completed");
            }
        }

        private async void btnRDmanuallist_Click(object sender, RoutedEventArgs e)
        {

            // get the main window
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            // start progress dialog controller
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Scraping",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await mw.ShowProgressAsync("Scraping Manual List from replacementdocs.com", "Initialising...", true, settings: mySettings);
            controller.SetCancelable(true);
            await Task.Delay(100);

            await Task.Run(() =>
            {
                RdScraper.ScrapeBasicDocsList(controller);
            });

            //MobyGames.ScrapeAllPlatformGames();
            // App app = ((App)Application.Current);

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await mw.ShowMessageAsync("replacementdocs.com manual link scraper", "Scraping Cancelled");
            }
            else
            {
                await mw.ShowMessageAsync("replacementdocs.com manual link scraper", "Scraping Completed");
            }
        }



        private void btnmobyPlatformListDumpToFile_Click(object sender, RoutedEventArgs e)
        {
            //MobyGames.DumpPlatformGamesToDisk();
        }

        private void btnCombine_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.BeginMerge(false, false, false);
        }

        private void btnGetManuals_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.ScrapeManuals();
        }

        private void btnCombineManual_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.BeginMerge(true, false, false);
        }

        private void btnCombineManualnonleven_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.BeginMerge(true, true, false);
        }

        private void btnCombineManualEverything_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.BeginMerge(false, false, true);
        }

        private void btnCombineGamesDatabaseOrgManuals_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.ParseGamesDatabaseOrgManuals();
        }

        private void btnParseRDList_Click(object sender, RoutedEventArgs e)
        {
            CreateMasterList j = new CreateMasterList();
            j.ParseReplacementDocsManuals();
        }

        /// <summary>
        /// Import all configs from disk into the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnConfigImportAll_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Import",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await this.ShowProgressAsync("Config Importer", "Importing all configs from disk...", true, settings: mySettings);
            controller.SetCancelable(true);
            controller.SetIndeterminate();
            await Task.Delay(1000);

            this.Dispatcher.Invoke(() =>
            {
                // get current active button
                ConfigsVisualHandler ch = new ConfigsVisualHandler();
                RadioButton rb = ch.FilterButtons.Where(a => a.IsChecked == true).Single();
                int ConfigId = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);

                ConfigImport ci = new ConfigImport();
                ci.ImportAll(controller);


                // update UI
                ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, ConfigId);
                ConfigNetplaySettings.LoadNetplaySettings(tbNetplayNick, slLocalPlayersValue, slConsoleLinesValue, slConsoleScaleValue, resOne, resTwo, resThree, resFour, resFive);
                //ConfigServerSettings.PopulateCustomServer(tbServerDesc, tbHostname, slServerPort, tbPassword, tbGameKey);
                //ConfigServerSettings.SetCustomDefault();

                SettingsVisualHandler.PopulateServers(lvServers);

                Task.Delay(500);

                if (rb != btnConfigLynx)
                    btnConfigLynx.IsChecked = true;
                else
                    btnConfigMd.IsChecked = true;

                Task.Delay(500);
                rb.IsChecked = true;
            });

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("Config Importer", "Config Import Cancelled");
            }
            else
            {
                await this.ShowMessageAsync("Config Importer", "Config Import Completed");
            }

            lblConfigStatus.Content = "All Configs Imported";
        }


        private void LoadConfigFromDisk_Click(object sender, RoutedEventArgs e)
        {

            MenuItem mi = sender as MenuItem;
            RadioButton rb = null;

            if (mi != null)
            {
                rb = ((ContextMenu)mi.Parent).PlacementTarget as RadioButton;
                rb.IsChecked = true;
                int ConfigId = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
                string sysCode = rb.Name.Replace("btnConfig", "").ToLower();
                
                ConfigImport ci = new ConfigImport();
                ci._ConfigBaseSettings = ConfigBaseSettings.GetConfig(ConfigId);
                ci.ImportSystemConfigFromDisk(null, GSystem.GetSystems().Where(a => a.systemCode == sysCode).FirstOrDefault());

                ci.SaveToDatabase();

                // update UI
                ConfigBaseSettings.LoadControlValues(ConfigWrapPanel, ConfigId);

                Task.Delay(500);

                if (rb != btnConfigLynx)
                    btnConfigLynx.IsChecked = true;
                else
                    btnConfigMd.IsChecked = true;

                Task.Delay(500);
                rb.IsChecked = true;

                // activate enabled systems
                ConfigsVisualHandler cvh = new ConfigsVisualHandler();
                cvh.ActivateEnabledSystems();

                lblConfigStatus.Content = sysCode.ToUpper() + " Config Imported";
               
            }
                
        }

        private void SaveConfigToDisk_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            int ConfigId = ConfigBaseSettings.GetConfigIdFromButtonName(rb.Name);
            string sysCode = rb.Name.Replace("btnConfig", "");
        }

        private void btnPJoyTest_Click(object sender, RoutedEventArgs e)
        {
            Controller.Start();
        }

        private async void btnReLink_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Import",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await this.ShowProgressAsync("Data Re-Linker", "Re-importing orphaned scraped data into games library main datagrid view", true, settings: mySettings);
            controller.SetCancelable(true);
            controller.SetIndeterminate();
            await Task.Delay(1000);

            this.Dispatcher.Invoke(() =>
            {
                GameListBuilder.ReLinkData();
            });

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("Data Re-Linker", "Linking Cancelled");
                GameListBuilder.UpdateFlag();
                GamesLibraryVisualHandler.RefreshGamesLibrary();
            }
            else
            {
                await this.ShowMessageAsync("Data Re-Linker", "Linking Completed");
                GamesLibraryVisualHandler.RefreshGamesLibrary();
                GameListBuilder.UpdateFlag();
            }
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private async void btnBuildFromDat_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Import",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await this.ShowProgressAsync("DAT Builder", "Scanning local DAT files...", true, settings: mySettings);
            controller.SetCancelable(true);
            controller.SetIndeterminate();
            await Task.Delay(1000);
            
            


            await Task.Run(() =>
            {
                controller.SetMessage("Loading MasterDAT JSON if present...");
                string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Data\System\DATMaster.json";
                List<DATMerge> Master = new List<DATMerge>();

                if (File.Exists(filepath))
                {
                    // DAT already exists - load it
                    Master = JsonConvert.DeserializeObject<List<DATMerge>>(File.ReadAllText(filepath));
                }


                controller.SetMessage("Parsing TOSEC DAT files");
                ToSecCollection tCol = new ToSecCollection();
                controller.SetMessage("Parsing NOINTRO DAT files");
                NoIntroCollection nCol = new NoIntroCollection();
                controller.SetMessage("Parsing TRURIP DAT files");
                TruRipCollection trCol = new TruRipCollection();
                controller.SetMessage("Parsing REDUMP DAT files");
                RedumpCollection rdCol = new RedumpCollection();

                // create a temp version of nCol
                List<NoIntroObject> nTemp = nCol.Data;

                // iterate through each ToSec entry
                

                string message = "Importing TOSEC releases to Master object\n";
                controller.SetMessage(message);

                int gameCount = 0;
                int romCount = 0;
                int duplicateCount = 0;

                foreach (var a in tCol.Data)
                {

                controller.SetMessage(message + "TOSEC Games Parsed: " + gameCount + "\nTOSEC Roms Parsed: " + romCount + "\nDuplicate Roms Skipped: " + duplicateCount);

                    //var cr = Master.Where(p => p.Roms.Any(x => x.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()));
                    var cr = from p in Master
                             where p.SystemId == a.SystemId &&
                             p.Roms.Any(x => x.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim())
                             select p;

                    if (cr.ToList().Count > 0)
                    {
                        duplicateCount++;
                        continue;
                    }
                    
                    // check whether GameName is present in Master
                    var n = (from r in Master
                             where (r.GameName.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " ") == a.Name.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " ") && r.SystemId == a.SystemId)
                             select r);
                    int mCount = n.ToList().Count();

                    if (mCount == 1)
                    {
                        /* one record returned - dig into this further */
                        var one = n.Single();

                        // search roms present for this game
                        var roms = (from rom in one.Roms
                                    where rom.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()
                                    select rom).ToList();

                        if (roms.ToList().Count == 0)
                        {
                            // rom with matching CRC was not found - add it
                            RomEntry r = new RomEntry();
                            r.RomName = a.RomName;
                            r.Country = a.Country;
                            r.Language = a.Language;
                            r.DevelopmentStatus = a.DevelopmentStatus;
                            r.OtherFlags = a.OtherFlags;
                            r.CloneOf = a.CloneOf;
                            r.Copyright = a.Copyright;
                            r.Size = a.Size;
                            r.CRC = a.CRC;
                            r.MD5 = a.MD5;
                            r.SHA1 = a.SHA1;
                            r.Year = a.Year;
                            r.Publisher = a.Publisher;

                            r.FromDAT = "T";

                            one.Roms.Add(r);
                            romCount++;

                        }
                        if (roms.Count == 1)
                        {
                            // rom was already found with matching CRC - do nothing
                            duplicateCount++;
                        }
                    }
                    if (mCount == 0)
                    {
                       
                        // no records return - add this record
                        DATMerge dm = new DATMerge();

                        dm.GameName = a.Name;
                        dm.Publisher = a.Publisher;
                        dm.Year = a.Year;
                        dm.SystemId = a.SystemId;

                        RomEntry r = new RomEntry();

                        r.RomName = a.RomName;
                        r.Country = a.Country;
                        r.Language = a.Language;
                        r.DevelopmentStatus = a.DevelopmentStatus;
                        r.OtherFlags = a.OtherFlags;
                        r.CloneOf = a.CloneOf;
                        r.Copyright = a.Copyright;
                        r.Size = a.Size;
                        r.CRC = a.CRC;
                        r.MD5 = a.MD5;
                        r.SHA1 = a.SHA1;
                        r.Year = a.Year;
                        r.Publisher = a.Publisher;

                        r.FromDAT = "T";

                        dm.Roms.Add(r);

                        Master.Add(dm);
                        gameCount++;
                        romCount++;
                    }
                    if (mCount > 1)
                    {
                        // duplicate found - add row to first
                    }
                }


                message = "Importing TRURIP releases to Master object\n";
                controller.SetMessage(message);

                gameCount = 0;
                romCount = 0;
                duplicateCount = 0;

                foreach (var a in trCol.Data)
                {

                    controller.SetMessage(message + "TRURIP Games Parsed: " + gameCount + "\nTRURIP Roms Parsed: " + romCount + "\nDuplicate Roms Skipped: " + duplicateCount);
                    //Thread.Sleep(1);

                    // first check whether MD5 is already present
                    /*
                    var cr = (from game in Master
                              from rom in game.Roms
                              where rom.MD5.ToUpper() == a.MD5.ToUpper()
                              select rom);
*/
                    var cr = Master.Where(p => p.Roms.Any(x => x.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()));

                    if (cr.ToList().Count > 0)
                    {
                        duplicateCount++;
                        continue;
                    }


                    // check whether GameName is present in Master
                    var n = (from r in Master
                             where (r.GameName.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " ") == a.Name.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " "))
                             && r.SystemId == a.SystemId
                             select r);
                    int mCount = n.ToList().Count();

                    if (mCount == 1)
                    {
                        /* one record returned - dig into this further */
                        var one = n.Single();

                        // search roms present for this game
                        var roms = (from rom in one.Roms
                                    where rom.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()
                                    select rom).ToList();

                        if (roms.ToList().Count == 0)
                        {
                            // rom with matching CRC was not found - add it
                            RomEntry r = new RomEntry();
                            r.RomName = a.RomName;
                            r.Country = a.Country;
                            r.Language = a.Language;
                            r.DevelopmentStatus = a.DevelopmentStatus;
                            r.OtherFlags = a.OtherFlags;
                            r.CloneOf = a.CloneOf;
                            r.Copyright = a.Copyright;
                            r.Size = a.Size;
                            r.CRC = a.CRC;
                            r.MD5 = a.MD5;
                            r.SHA1 = a.SHA1;
                            r.Year = a.Year;
                            r.Publisher = a.Publisher;

                            r.FromDAT = "TR";

                            one.Roms.Add(r);
                            romCount++;

                        }
                        if (roms.Count == 1)
                        {
                            // rom was already found with matching CRC - do nothing
                            duplicateCount++;
                        }
                    }
                    if (mCount == 0)
                    {

                        // no records return - add this record
                        DATMerge dm = new DATMerge();

                        dm.GameName = a.Name;
                        dm.Publisher = a.Publisher;
                        dm.Year = a.Year;
                        dm.SystemId = a.SystemId;

                        RomEntry r = new RomEntry();

                        r.RomName = a.RomName;
                        r.Country = a.Country;
                        r.Language = a.Language;
                        r.DevelopmentStatus = a.DevelopmentStatus;
                        r.OtherFlags = a.OtherFlags;
                        r.CloneOf = a.CloneOf;
                        r.Copyright = a.Copyright;
                        r.Size = a.Size;
                        r.CRC = a.CRC;
                        r.MD5 = a.MD5;
                        r.SHA1 = a.SHA1;
                        r.Year = a.Year;
                        r.Publisher = a.Publisher;

                        r.FromDAT = "TR";

                        dm.Roms.Add(r);

                        Master.Add(dm);
                        gameCount++;
                        romCount++;
                    }
                    if (mCount > 1)
                    {
                        // duplicate discovered this shoudlnt happen
                        //do nothing
                    }
                }



                message = "Importing NOINTRO releases to Master object\n";
                controller.SetMessage(message);

                gameCount = 0;
                romCount = 0;
                duplicateCount = 0;

                foreach (var a in nCol.Data)
                {

                    controller.SetMessage(message + "NOINTRO Games Parsed: " + gameCount + "\nNOINTRO Roms Parsed: " + romCount + "\nDuplicate Roms Skipped: " + duplicateCount);
                    //Thread.Sleep(1);

                    // first check whether MD5 is already present
                    /*
                    var cr = (from game in Master
                              from rom in game.Roms
                              where rom.MD5.ToUpper() == a.MD5.ToUpper()
                              select rom);
*/
                    var cr = Master.Where(p => p.Roms.Any(x => x.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()));

                    if (cr.ToList().Count > 0)
                    {
                        duplicateCount++;
                        continue;
                    }
                        

                    // check whether GameName is present in Master
                    var n = (from r in Master
                             where (r.GameName.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " ") == a.Name.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " "))
                             && r.SystemId == a.SystemId
                             select r);
                    int mCount = n.ToList().Count();

                    if (mCount == 1)
                    {
                        /* one record returned - dig into this further */
                        var one = n.Single();

                        // search roms present for this game
                        var roms = (from rom in one.Roms
                                    where rom.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()
                                    select rom).ToList();

                        if (roms.ToList().Count == 0)
                        {
                            // rom with matching CRC was not found - add it
                            RomEntry r = new RomEntry();
                            r.RomName = a.RomName;
                            r.Country = a.Country;
                            r.Language = a.Language;
                            r.DevelopmentStatus = a.DevelopmentStatus;
                            r.OtherFlags = a.OtherFlags;
                            r.CloneOf = a.CloneOf;
                            r.Copyright = a.Copyright;
                            r.Size = a.Size;
                            r.CRC = a.CRC;
                            r.MD5 = a.MD5;
                            r.SHA1 = a.SHA1;
                            r.Year = a.Year;
                            r.Publisher = a.Publisher;

                            r.FromDAT = "N";

                            one.Roms.Add(r);
                            romCount++;

                        }
                        if (roms.Count == 1)
                        {
                            // rom was already found with matching CRC - do nothing
                            duplicateCount++;
                        }
                    }
                    if (mCount == 0)
                    {

                        // no records return - add this record
                        DATMerge dm = new DATMerge();

                        dm.GameName = a.Name;
                        dm.Publisher = a.Publisher;
                        dm.Year = a.Year;
                        dm.SystemId = a.SystemId;

                        RomEntry r = new RomEntry();

                        r.RomName = a.RomName;
                        r.Country = a.Country;
                        r.Language = a.Language;
                        r.DevelopmentStatus = a.DevelopmentStatus;
                        r.OtherFlags = a.OtherFlags;
                        r.CloneOf = a.CloneOf;
                        r.Copyright = a.Copyright;
                        r.Size = a.Size;
                        r.CRC = a.CRC;
                        r.MD5 = a.MD5;
                        r.SHA1 = a.SHA1;
                        r.Year = a.Year;
                        r.Publisher = a.Publisher;

                        r.FromDAT = "N";

                        dm.Roms.Add(r);

                        Master.Add(dm);
                        gameCount++;
                        romCount++;
                    }
                    if (mCount > 1)
                    {
                        // duplicate discovered this shoudlnt happen
                        //do nothing
                    }
                }


                message = "Importing REDUMP releases to Master object\n";
                controller.SetMessage(message);

                gameCount = 0;
                romCount = 0;
                duplicateCount = 0;

                foreach (var a in rdCol.Data)
                {

                    controller.SetMessage(message + "REDUMP Games Parsed: " + gameCount + "\nREDUMP Roms Parsed: " + romCount + "\nDuplicate Roms Skipped: " + duplicateCount);
                    //Thread.Sleep(1);

                    // first check whether MD5 is already present
                    /*
                    var cr = (from game in Master
                              from rom in game.Roms
                              where rom.MD5.ToUpper() == a.MD5.ToUpper()
                              select rom);
*/
                    var cr = Master.Where(p => p.Roms.Any(x => x.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()));

                    if (cr.ToList().Count > 0)
                    {
                        duplicateCount++;
                        continue;
                    }


                    // check whether GameName is present in Master
                    var n = (from r in Master
                             where (r.GameName.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " ") == a.Name.ToUpper().Replace(":", "").Replace("-", "").Replace("'", "").Replace(",", "").Replace("  ", " "))
                             && r.SystemId == a.SystemId
                             select r);
                    int mCount = n.ToList().Count();

                    if (mCount == 1)
                    {
                        /* one record returned - dig into this further */
                        var one = n.Single();

                        // search roms present for this game
                        var roms = (from rom in one.Roms
                                    where rom.MD5.ToUpper().Trim() == a.MD5.ToUpper().Trim()
                                    select rom).ToList();

                        if (roms.ToList().Count == 0)
                        {
                            // rom with matching CRC was not found - add it
                            RomEntry r = new RomEntry();
                            r.RomName = a.RomName;
                            r.Country = a.Country;
                            r.Language = a.Language;
                            r.DevelopmentStatus = a.DevelopmentStatus;
                            r.OtherFlags = a.OtherFlags;
                            r.CloneOf = a.CloneOf;
                            r.Copyright = a.Copyright;
                            r.Size = a.Size;
                            r.CRC = a.CRC;
                            r.MD5 = a.MD5;
                            r.SHA1 = a.SHA1;
                            r.Year = a.Year;
                            r.Publisher = a.Publisher;

                            r.FromDAT = "R";

                            one.Roms.Add(r);
                            romCount++;

                        }
                        if (roms.Count == 1)
                        {
                            // rom was already found with matching CRC - do nothing
                            duplicateCount++;
                        }
                    }
                    if (mCount == 0)
                    {

                        // no records return - add this record
                        DATMerge dm = new DATMerge();

                        dm.GameName = a.Name;
                        dm.Publisher = a.Publisher;
                        dm.Year = a.Year;
                        dm.SystemId = a.SystemId;

                        RomEntry r = new RomEntry();

                        r.RomName = a.RomName;
                        r.Country = a.Country;
                        r.Language = a.Language;
                        r.DevelopmentStatus = a.DevelopmentStatus;
                        r.OtherFlags = a.OtherFlags;
                        r.CloneOf = a.CloneOf;
                        r.Copyright = a.Copyright;
                        r.Size = a.Size;
                        r.CRC = a.CRC;
                        r.MD5 = a.MD5;
                        r.SHA1 = a.SHA1;
                        r.Year = a.Year;
                        r.Publisher = a.Publisher;

                        r.FromDAT = "R";

                        dm.Roms.Add(r);

                        Master.Add(dm);
                        gameCount++;
                        romCount++;
                    }
                    if (mCount > 1)
                    {
                        // duplicate discovered this shoudlnt happen
                        //do nothing
                    }
                }


                // save to json file
                controller.SetMessage("Saving MasterDAT JSON...");
                filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Data\System\DATMaster.json";
                string output = JsonConvert.SerializeObject(Master, Formatting.Indented);
                File.WriteAllText(filepath, output);
            });

            


            this.Dispatcher.Invoke(() =>
            {                
                            
            });

            
            this.Dispatcher.Invoke(() =>
            {
                
            });

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("DAT Builder", "Linking Cancelled");
                GameListBuilder.UpdateFlag();
                GamesLibraryVisualHandler.RefreshGamesLibrary();
            }
            else
            {
                await this.ShowMessageAsync("DAT Builder", "Linking Completed");
                GameListBuilder.UpdateFlag();
                GamesLibraryVisualHandler.RefreshGamesLibrary();                
            }
        }


        private async void btnmatchDATyears_Click(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                NegativeButtonText = "Cancel Import",
                AnimateShow = false,
                AnimateHide = false
            };
            var controller = await this.ShowProgressAsync("DAT Builder", "Resolving missing information", true, settings: mySettings);
            controller.SetCancelable(true);
            controller.SetIndeterminate();
            await Task.Delay(1000);


            string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Data\System\DATMaster.json";
            string json = File.ReadAllText(filepath);
            List<DATMerge> dm = JsonConvert.DeserializeObject<List<DATMerge>>(json);

            string gdbjson = File.ReadAllText(@"..\..\Data\System\TheGamesDB.json");
            List < GDBPlatformGame > gdb = JsonConvert.DeserializeObject<List<GDBPlatformGame>>(gdbjson);

            await Task.Run(() =>
            {
                int skipped = 0;
                int matched = 0;
                int unmatched = 0;

                // match exactly gamesdb games and get date
                int length = dm.Count();

                for (int i = 0; i < length; i++)
                {
                    controller.SetMessage("Parsing MASTER DAT file..\n\nSkipped (already matched): " + skipped + "\nMatched (against tgdb list): " + matched + "\nUnmatched: " + unmatched);

                    if (dm[i].Year != null)
                    {
                        // year is already present
                        skipped++;
                        continue;
                    }

                    // now do an exact name match - first get the systemid
                    int systemId = dm[i].SystemId;

                    // filter the gdb object
                    var g = gdb.Where(a => a.SystemId == systemId);

                    var result = (from entry in g
                                  where entry.GameTitle.Trim().ToLower().Replace(":", "").Replace("'", "").Replace("-", "").Replace("&amp;", "").Replace("and", "").Replace("&", "").Replace("  ", " ") 
                                  == 
                                  dm[i].GameName.Trim().ToLower().Replace(":", "").Replace("'", "").Replace("-", "").Replace("&amp;", "").Replace("and", "").Replace("&", "").Replace("  ", " ")
                                  select entry).ToList();

                    if (result.Count() > 0)
                    {
                        if (result.First().ReleaseDate == null)
                        {
                            unmatched++;
                            continue;
                        }
                            
                        string date = result.First().ReleaseDate;
                        // split based on /

                        string[] s = date.Split('/');
                        if (s.Length < 3)
                        {
                            // incorrect date format unless the string is only 4 characters
                            if (s.Length == 1)
                            {
                                if (s[0].Length == 4)
                                {
                                    dm[i].Year = s[0];
                                    continue;
                                }
                                continue;
                            }
                            continue;
                        }

                        dm[i].Year = s[2];
                        matched++;
                        continue;
                    }
                    else
                    {
                        unmatched++;
                    }
                }

                
            });

            await Task.Delay(5000);
            controller.SetMessage("Saving master DAT");
            string output = JsonConvert.SerializeObject(dm, Formatting.Indented);
            File.WriteAllText(filepath, output);

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("DAT Builder", "Linking Cancelled");
                GameListBuilder.UpdateFlag();
                GamesLibraryVisualHandler.RefreshGamesLibrary();
            }
            else
            {
                await this.ShowMessageAsync("DAT Builder", "Linking Completed");
                GameListBuilder.UpdateFlag();
                GamesLibraryVisualHandler.RefreshGamesLibrary();
            }
        }






        private void chkStreamDisk_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkStreamDisk_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkStreamTwitch_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void chkStreamTwitch_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnPathFfmpeg_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPathOutputFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void chkForceOverwrite_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkForceOverwrite_Unchecked(object sender, RoutedEventArgs e)
        {

        }


        private void columnHeaderDGGAMES_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            
            if (columnHeader != null)
            {
                string columnName = ((DataGridColumnHeader)sender).Content.ToString();

                App _App = ((App)Application.Current);

                ListSortDirection sortDirection = (((DataGridColumnHeader)sender).SortDirection != ListSortDirection.Ascending) ?
                                ListSortDirection.Ascending : ListSortDirection.Descending;
                _App.GamesList.SortColumnName = columnName;
                _App.GamesList.SortDirection = sortDirection;
            }
        }

        private void btnViewMednafenLogFile_Click(object sender, RoutedEventArgs e)
        {
            // get path to mednafen folder
            Paths p = Paths.GetPaths();
            string folderPath = p.mednafenExe;
            string logPath = folderPath + "\\" + "stdout.txt";

            // test whether log exists
            if (!File.Exists(logPath))
                return;

            // open log in default editor
            FileAssociation.OpenTxtFileInDefaultViewer(logPath);

            

        }

        private void btnViewMednafenExFile_Click(object sender, RoutedEventArgs e)
        {            
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Exceptions.log";

            // test whether log exists
            if (!File.Exists(logPath))
                return;

            // open log in default editor
            FileAssociation.OpenTxtFileInDefaultViewer(logPath);

        }

        private void comboPsxRecalculate_Click(object sender, RoutedEventArgs e)
        {
            List<CheckBox> chks = new List<CheckBox>
            {
                comboPsx1,
                comboPsx2,
                comboPsx3,
                comboPsx4,
                comboPsx5,
                comboPsx6,
                comboPsx7,
                comboPsx8,
                comboPsx9,
                comboPsx10,
                comboPsx11,
                comboPsx12,
                comboPsx13,
                comboPsx14,
                comboPsx15,
                comboPsx16
            };

            // get all checked checkboxes
            var chked = (from a in chks
                         where a.IsChecked == true
                         select a).ToList();

            string hex = CalculateCombo(chked);

            // set text box 
            cfg_psx__input__analog_mode_ct__compare.Text = hex;

        }

        public void CalculateComboChecks(string hexValue)
        {
            // get list of checkboxes
            List<CheckBox> chks = new List<CheckBox>
            {
                comboPsx1,
                comboPsx2,
                comboPsx3,
                comboPsx4,
                comboPsx5,
                comboPsx6,
                comboPsx7,
                comboPsx8,
                comboPsx9,
                comboPsx10,
                comboPsx11,
                comboPsx12,
                comboPsx13,
                comboPsx14,
                comboPsx15,
                comboPsx16
            };

            int value = int.Parse(hexValue.TrimStart('0').TrimStart('x'), System.Globalization.NumberStyles.HexNumber);
            for (int i = 16; i > 0; i--)
            {
                int testValue = 1 * Convert.ToInt32(Math.Pow(2, Convert.ToDouble(i - 1)));
                if (value / testValue == 1)
                {
                    // checkbox that ends with 'i' needs to be ticked - 
                    CheckBox cb = chks.Where(a => a.Name.Contains(i.ToString())).First();
                    cb.IsChecked = true;

                    //now set remainder
                    value = value % testValue;
                }
            }              
        }


        public static string CalculateCombo(List<CheckBox> chkboxes)
        {
            int total = 0;

            foreach (var c in chkboxes)
            {
                string name = c.Name.Replace("comboPsx", "");

                int value = Convert.ToInt32(name);

                int calc = 1;
                for (int i = 1; i <= value; i++)
                {
                    if (i == 1)
                    {
                        calc = 1;
                        continue;
                    }

                    calc = calc * 2;
                }

                total += calc;
            }

            // convert to hex string
            string hexValue = "0x" + total.ToString("X4");
            return hexValue;
            
        }

        private void btnGenerateToolTips_Click(object sender, RoutedEventArgs e)
        {
            var r = ConfigToolTips.GetDocumentationStrings();
        }

        private void btnPopToolTips_Click(object sender, RoutedEventArgs e)
        {
            ConfigToolTips.SetToolTips(1);
        }

        private void menuQuitMedLaunch_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TopMenu_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void manualLocateLaunchGame_Click(object sender, RoutedEventArgs e)
        {
            // popup select game file dialog
            string filePath = GameLauncher.SelectGameFile();

            // check whether path is null
            if (filePath == null)
                return;

            // check whether file exists
            if (!File.Exists(filePath))
                return;

            // Attempt to launch game using Mednafen config settings that are on disk (not parsing any MedLaunch command line parameters)
            string medExe = Paths.GetPaths().mednafenExe + "\\mednafen.exe";

            // check whether minimise to taskbar option is checked
            bool taskbar = this.ShowInTaskbar;
            if (GlobalSettings.Min2TaskBar() == true)
            {
                this.ShowInTaskbar = true;
                this.WindowState = WindowState.Minimized;
            }


            System.Diagnostics.Process gProcess = new System.Diagnostics.Process();
            gProcess.StartInfo.UseShellExecute = true;
            gProcess.StartInfo.RedirectStandardOutput = false;
            //gProcess.StartInfo.WorkingDirectory = "\"" + Paths.GetPaths().mednafenExe + "\"";
            gProcess.StartInfo.FileName = medExe;
            gProcess.StartInfo.CreateNoWindow = false;
            gProcess.StartInfo.Arguments = "\"" + filePath + "\"";
            gProcess.Start();
            gProcess.WaitForExit();


            if (GlobalSettings.Min2TaskBar() == true)
            {
                this.ShowInTaskbar = taskbar;
                this.WindowState = WindowState.Normal;
            }

            

        }

        private async void MenuItemColumn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            await mw.ShowChildWindowAsync(new LibraryColumnChooser()
            {
                IsModal = true,
                AllowMove = false,
                Title = "Games Library Column Visibility Settings",
                CloseOnOverlay = false,
                ShowCloseButton = false
            }, RootGrid);
        }

        private void tbSettings_Paths_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.GamePaths);
        }

        private void tbSettings_Global_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.GlobalSettings);
        }

        private void tbSettings_BiosPaths_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.BiosPaths);
        }

        private void comboSettings_Global_DropDownClosed(object sender, EventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.GlobalSettings);
        }

        private void slSettings_Global_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.GlobalSettings);
        }

        private void tbSettings_MednafenPaths_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.MednafenPaths);
        }

        private void tbSettings_Netplay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.NetplaySettings);
        }

        private void tbSettings_Server_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.ServerSettings);
        }

        private void slSettings_Server_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.ServerSettings);
        }

        private void slSettings_Netplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.NetplaySettings);
        }

        private void rbSettings_Netplay_Checked(object sender, RoutedEventArgs e)
        {
            if (SettingsDirtyFlag == true)
                ConfigsVisualHandler.SaveSettings(SettingGroup.NetplaySettings);
        }

        private async void AuditScrapedData_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            await mw.ShowChildWindowAsync(new ScrapedDataAudit()
            {
                IsModal = true,
                AllowMove = false,
                Title = "Scraped Data Folder Audit",
                CloseOnOverlay = false,
                ShowCloseButton = false
            }, RootGrid);
        }

        private void lvServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var row = (ServersListView)lvServers.SelectedItem;

            if (row == null)
            {
                btnServersSelect.Visibility = Visibility.Collapsed;
                btnServersDelete.Visibility = Visibility.Collapsed;
                btnServersSaveEdit.Visibility = Visibility.Collapsed;

                tbServerDesc.Text = null;
                tbHostname.Text = null;
                slServerPort.Value = 4046;
                tbPassword.Text = null;
                tbGameKey.Text = null;
            }
            else
            {
                btnServersSelect.Visibility = Visibility.Visible;
                btnServersDelete.Visibility = Visibility.Visible;
                btnServersSaveEdit.Visibility = Visibility.Visible;

                tbServerDesc.Text = row.Name;
                tbHostname.Text = row.Host;
                tbPassword.Text = row.Password;
                tbGameKey.Text = row.Gamekey;
                if (row.Port > 0)
                {
                    slServerPort.Value = Convert.ToDouble(row.Port);
                }
                
            }
        }

        private void btnServersSelect_Click(object sender, RoutedEventArgs e)
        {
            var row = (ServersListView)lvServers.SelectedItem;

            if (row == null)
            {
                MessageBox.Show("No Server Selected!");
            }
            else
            {
                GlobalSettings gs = GlobalSettings.GetGlobals();
                gs.serverSelected = row.ID;
                GlobalSettings.SetGlobals(gs);

                SettingsVisualHandler.PopulateServers(lvServers);
            }
        }

        private void btnServersAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tbHostname.Text == null || tbHostname.Text == "" || tbHostname.Text.Trim() == "")
            {
                // hostname has not been entered
                MessageBox.Show("You must provide a Hostname or IP Address");
                return;
            }

            // get the server list
            var dbServers = ConfigServerSettings.GetServers();

            // build a server object
            ConfigServerSettings s = new ConfigServerSettings();

            // check for full match
            var chk = from a in dbServers
                                       where
                                       a.ConfigServerDesc == tbServerDesc.Text &&
                                       a.netplay__gamekey == tbGameKey.Text &&
                                       a.netplay__host == tbHostname.Text &&
                                       a.netplay__password == tbPassword.Text &&
                                       a.netplay__port == Convert.ToInt32(slServerPort.Value)
                                       select a;
            if (chk.Count() > 1)
            {
                MessageBox.Show("This server and associated settings already exists!");
                return;
            }

            // save the server
            s.ConfigServerDesc = tbServerDesc.Text;
            s.netplay__gamekey = tbGameKey.Text;
            s.netplay__host = tbHostname.Text;
            s.netplay__password = tbPassword.Text;
            s.netplay__port = Convert.ToInt32(slServerPort.Value);

            ConfigServerSettings.SaveToDatabase(s);

            SettingsVisualHandler.PopulateServers(lvServers);
        }

        private void btnServersDelete_Click(object sender, RoutedEventArgs e)
        {
            var row = (ServersListView)lvServers.SelectedItem;

            if (row == null)
            {
                MessageBox.Show("No Server Selected!");
                return;
            }
            
            if (row.Selected == true)
            {
                MessageBox.Show("Unable to delete because this is the current default server.\nSet another server to default (the 'use selected server button') and then try again");
                return;
            }

            // obtain ID and delete selected server from the database
            ConfigServerSettings.DeleteServer(row.ID);
            SettingsVisualHandler.PopulateServers(lvServers);
        }

        private void btnServersSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            // get the selected ID
            var row = (ServersListView)lvServers.SelectedItem;

            if (row == null)
            {
                MessageBox.Show("No Server Selected!");
                return;
            }

            int id = row.ID;

            // create a new object
            ConfigServerSettings s = new ConfigServerSettings();

            s.ConfigServerId = id;
            s.ConfigServerDesc = tbServerDesc.Text;
            s.netplay__gamekey = tbGameKey.Text;
            s.netplay__host = tbHostname.Text;
            s.netplay__password = tbPassword.Text;
            s.netplay__port = Convert.ToInt32(slServerPort.Value);

            // update the record in the database
            ConfigServerSettings.SaveToDatabase(s);

            SettingsVisualHandler.PopulateServers(lvServers);
        }

        private async void medCoreVisibility_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            await mw.ShowChildWindowAsync(new MednafenCoreVisibility()
            {
                IsModal = true,
                AllowMove = false,
                Title = "Manage Emulated System Visibility",
                CloseOnOverlay = false,
                ShowCloseButton = false
            }, RootGrid);
        }

        private void btnForgetSystemPosition_Click(object sender, RoutedEventArgs e)
        {
            GlobalSettings.ResetAllSysWindowPositions();
        }

        private void disctest_Click(object sender, RoutedEventArgs e)
        {
            string serial1 = DiscUtils.GetPSXSerial(@"G:\_Emulation\PSX\iso\Final Fantasy 9 [PSX][PAL]\Final Fantasy 9 CD1.img");
            string serial2 = DiscUtils.GetPSXSerial(@"G:\_Emulation\PSX\iso\Final Fantasy 9 [PSX][PAL]\Final Fantasy 9 CD2.img");
            string serial3 = DiscUtils.GetPSXSerial(@"G:\_Emulation\PSX\iso\Final Fantasy 9 [PSX][PAL]\Final Fantasy 9 CD3.img");
            string serial4 = DiscUtils.GetPSXSerial(@"G:\_Emulation\PSX\iso\Final Fantasy 1 (Origins)(E) [SLES-04034]\Final Fantasy Origins - Final Fantasy (E) [SLES-04034].bin");
            string serial5 = DiscUtils.GetPSXSerial(@"G:\_Emulation\PSX\iso\Alien Trilogy (E) [SLES-00101]\Alien Trilogy (E) [SLES-00101].iso");
        }
    }
    /*
    public class SliderIgnoreDelta : Slider
    {
        protected override void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            // Do nothing
        }
    }
    */

    

}
