﻿using Avalonia.Controls.ApplicationLifetimes;
using Maize;
using Maize.Models.ApplicationSpecific;
using MaizeUI.Views;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using Avalonia;
using Maize.Helpers;
using System.Net.Http;
using static Maize.Models.ApplicationSpecific.Constants;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace MaizeUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; set; }

        public List<string> Networks { get; set; } 

        public string selectedNetwork;

        public string SelectedNetwork
        {
            get => selectedNetwork;
            set => this.RaiseAndSetIfChanged(ref selectedNetwork, value);
        }

        public ReactiveCommand<Unit, Unit> VerifyAppSettingsCommand { get; }

        public MainWindowViewModel()
        {
            Greeting = "Welcome to Maize!";
            Networks = new List<string> { "mainnet", "testnet" };
            SelectedNetwork = Networks[0];
            VerifyAppSettingsCommand = ReactiveCommand.Create(VerifyAppSettings);
        }


        private async void VerifyAppSettings()
        {
            string appSettingsEnvironment = $"{Constants.BaseDirectory}{Constants.EnvironmentPath}{selectedNetwork}appsettings.json";
             IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile(appSettingsEnvironment)
            .AddEnvironmentVariables()
            .Build();
            Settings settings = config.GetRequiredSection("Settings").Get<Settings>();
            var environment = Constants.GetNetworkConfig(settings.Environment);
            if (settings.LoopringAccountId == 1234 || settings.LoopringApiKey == "asdfasdfasdfasdfasdfasdf")
            {
                await ShowAppSettingsDialog($"Please verify your {selectedNetwork}appsettings.json located at: {appSettingsEnvironment} is correct!");
            }
            else
            {
                ILoopringService loopringService = new LoopringService(environment.Url);
                var signedMessage = EDDSAHelper.EddsaSignUrl(settings.LoopringPrivateKey, HttpMethod.Get, new List<(string Key, string Value)>() { ("accountId", settings.LoopringAccountId.ToString()) }, null, "api/v3/apiKey", environment.Url);
                var apiKey = await loopringService.GetApiKey(settings.LoopringAccountId, signedMessage);
                if (apiKey != settings.LoopringApiKey)
                {
                    await ShowAppSettingsDialog($"Please check your LoopringApiKey in {selectedNetwork}appsettings.json file located at: {appSettingsEnvironment} is correct.");
                }
                else
                {
                    ShowMainMenuDialog(settings, environment);
                }
            }
        }

        private async Task ShowAppSettingsDialog(string notice)
        {
            var dialog = new AppsettingsNoticeWindow();
            dialog.DataContext = new AppsettingsNoticeWindowViewModel
            {
                Notice = notice
            };
            dialog.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner;
            await dialog.ShowDialog((Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow);
        }

        private void ShowMainMenuDialog(Settings settings, Constants.Environment environment)
        {
            var dialog = new MainMenuWindow();
            dialog.DataContext = new MainMenuWindowViewModel
            {
                Settings = settings,
                Environment = environment
            };
            dialog.WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner;
            var mainWindow = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow;
            (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow = dialog;
            dialog.Show();
            mainWindow.Close();
        }
    }
}
