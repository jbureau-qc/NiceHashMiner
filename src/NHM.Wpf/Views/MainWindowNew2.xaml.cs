﻿using NHM.Wpf.ViewModels;
using NHM.Wpf.Views.Common;
using NHM.Wpf.Views.Common.NHBase;
using NHMCore;
using NHMCore.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NHM.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindowNew2.xaml
    /// </summary>
    public partial class MainWindowNew2 : NHMMainWindow
    {
        private readonly MainVM _vm;
        private bool _miningStoppedOnClose;

        public MainWindowNew2()
        {
            InitializeComponent();

            _vm = this.AssertViewModel<MainVM>();

            Translations.LanguageChanged += (s, e) => WindowUtils.Translate(this);

            WindowUtils.InitWindow(this);

            LoadingBar.Visibility = Visibility.Visible;
        }

        #region Start-Loaded/Closing
        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await MainWindow_OnLoadedTask();
        }

        // just in case we add more awaits this signature will await all of them
        private async Task MainWindow_OnLoadedTask()
        {
            WindowUtils.SetForceSoftwareRendering(this);
            try
            {
                await _vm.InitializeNhm(LoadingBar.StartupLoader);
            }
            finally
            {
                LoadingBar.Visibility = Visibility.Collapsed;
                // Re-enable managed controls
                IsEnabled = true;
                SetTabButtonsEnabled();
            }
        }

        private async void MainWindow_OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await MainWindow_OnClosingTask(e);
        }

        private async Task MainWindow_OnClosingTask(System.ComponentModel.CancelEventArgs e)
        {
            // Only ever try to prevent closing once
            if (_miningStoppedOnClose) return;

            _miningStoppedOnClose = true;
            //e.Cancel = true;
            IsEnabled = false;
            //await _vm.StopMining();
            await ApplicationStateManager.BeforeExit();
            //Close();
        }
        #endregion Start-Loaded/Closing

        protected override void OnTabSelected(ToggleButtonType tabType)
        {
            var tabName = tabType.ToString();
            foreach (TabItem tab in MainTabs.Items)
            {
                if (tabName.Contains(tab.Name))
                {
                    MainTabs.SelectedItem = tab;
                    break;
                }
            }
        }

        #region Minimize to tray stuff
        private void CloseMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TaskbarIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized) // TODO && config min to tray
                Hide();
        }
        #endregion Minimize to tray stuff
    }
}
