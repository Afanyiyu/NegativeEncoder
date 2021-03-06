﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace NegativeEncoder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //初始化（阶段2）
            DataContext = new
            {
                AppContext.Version,
                AppContext.PresetContext
            };
            StatusBar.DataContext = AppContext.Status;
            FunctionTabs.DataContext = AppContext.PresetContext;
            TaskQueueListBox.ItemsSource = AppContext.EncodingContext.TaskQueue;

            AppContext.Status.MainStatus = "载入系统配置...";
            AppContext.Config = SystemOptions.SystemOption.ReadOption<SystemOptions.Config>().GetAwaiter().GetResult(); //读取全局配置
            Presets.PresetProvider.LoadPresetAutoSave().GetAwaiter().GetResult(); //读取当前预设

            Presets.PresetProvider.InitPresetAutoSave(PresetMenuItems); //初始化预设自动保存

            AutoCheckUpdateAfterStartupMenuItem.IsChecked = AppContext.Config.AutoCheckUpdate;

            OpenNewVersionReleasePageMenuItem.DataContext = AppContext.Version;

            if (AppContext.Config.AutoCheckUpdate)
            {
                CheckUpdateMenuItem_Click(sender, e);
            }

            AppContext.Status.MainStatus = "就绪";
        }

        private void ImportVideoMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFileList.ImportVideoAction(sender, e);
        }

        private void ClearFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFileList.ClearFileList(sender, e);
        }

        private void ExitAppMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AutoCheckUpdateAfterStartupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AppContext.Config.AutoCheckUpdate = AutoCheckUpdateAfterStartupMenuItem.IsChecked;
            SystemOptions.SystemOption.SaveOption(AppContext.Config).GetAwaiter().GetResult();
        }

        private void CheckUpdateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await About.CheckUpdate.Check();
            });
        }

        private void OpenNewVersionReleasePageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var url = AppContext.Version.UpdateVersionLinkUrl;
            if (!string.IsNullOrEmpty(url))
            {
                Utils.OpenBrowserViewLink.OpenUrl(url);
            }
        }

        private void OpenAboutWindowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new About.AboutWindow
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            aboutWindow.Show();
        }

        private void NewPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Presets.PresetProvider.NewPreset(this);
        }

        private void SavePresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _ = Presets.PresetProvider.SavePreset(PresetMenuItems);
        }

        private void SaveAsPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var oldName = AppContext.PresetContext.CurrentPreset.PresetName;

            var newNameWindow = new Presets.PresetReName(oldName)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            if (newNameWindow.ShowDialog() == true)
            {
                var newName = newNameWindow.NameBox.Text;

                _ = Presets.PresetProvider.SaveAsPreset(PresetMenuItems, newName);
            }
        }

        private void RenamePresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var oldName = AppContext.PresetContext.CurrentPreset.PresetName;

            var newNameWindow = new Presets.PresetReName(oldName)
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this
            };
            if (newNameWindow.ShowDialog() == true)
            {
                var newName = newNameWindow.NameBox.Text;

                _ = Presets.PresetProvider.RenamePreset(PresetMenuItems, newName);
            }
        }

        private void DeletePresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _ = Presets.PresetProvider.DeletePreset(PresetMenuItems);
        }

        private void ExportPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                DefaultExt = "json",
                Filter = "预设配置文件（JSON） (*.json)|*.json|所有文件(*.*)|*.*"
            };

            if (sfd.ShowDialog() == true)
            {
                _ = Presets.PresetProvider.ExportPreset(sfd.FileName);
            }
        }

        private void ImportPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "预设配置文件（JSON） (*.json)|*.json|所有文件(*.*)|*.*"
            };

            if (ofd.ShowDialog() == true)
            {
                _ = Presets.PresetProvider.ImportPreset(PresetMenuItems, ofd.FileName);
            }
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var source = ((ListBoxItem)sender).Content as EncodingTask.EncodingTask;

            var taskDetailWindow = new EncodingTask.TaskDetailWindow
            {
                Owner = this,
                DataContext = source
            };

            taskDetailWindow.Show();
        }

        private void TaskScheduleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EncodingTask.TaskProvider.Schedule();
        }

        private void EncodeContextMenuOpenDetailMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = TaskQueueListBox.SelectedItem as EncodingTask.EncodingTask;
            var taskDetailWindow = new EncodingTask.TaskDetailWindow
            {
                Owner = this,
                DataContext = source
            };

            taskDetailWindow.Show();
        }

        private void EncodeContextMenuBrowseOutputDirMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var source = TaskQueueListBox.SelectedItem as EncodingTask.EncodingTask;

            if (!string.IsNullOrEmpty(source.Output))
            {
                var psi = new ProcessStartInfo("explorer.exe")
                {
                    Arguments = "/e,/select," + source.Output
                };
                Process.Start(psi);
            }
        }

        private void OpenNEENCToolsCmdMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Utils.CmdTools.SaveCmdTools.SaveOpenBat();
            }
            catch (UnauthorizedAccessException)
            {
                var psi = new ProcessStartInfo(AppContext.EncodingContext.AppSelf)
                {
                    Arguments = $"--runFunc SaveCmdTools --baseDir \"{AppContext.EncodingContext.BaseDir}\"",
                    UseShellExecute = true,
                    Verb = "RunAs"
                };
                Process.Start(psi);
            }
            finally
            {
                Utils.CmdTools.SaveCmdTools.OpenCmdTools();
            }


        }

        private void NEToolsInstallMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo(AppContext.EncodingContext.AppSelf)
            {
                Arguments = $"--runFunc InstallCmdTools --baseDir \"{AppContext.EncodingContext.BaseDir}\"",
                UseShellExecute = true,
                Verb = "RunAs"
            };
            Process.Start(psi);
        }

        private void NEToolsRemoveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo(AppContext.EncodingContext.AppSelf)
            {
                Arguments = $"--runFunc UninstallCmdTools --baseDir \"{AppContext.EncodingContext.BaseDir}\"",
                UseShellExecute = true,
                Verb = "RunAs"
            };
            Process.Start(psi);
        }
    }
}
