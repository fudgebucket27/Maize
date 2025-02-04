﻿namespace Maize.Models.ApplicationSpecific
{
    public class Constants
    {
        public const int NftType = 0;
        public const string MaxFeeToken = "LRC";
        public const string AccessLogo = "0x08dccae9dac82c69e6836977c932bb55e608d548d19e95addee8817f7edb5f8d";
        public const string AccessPremiumContent = "0x22829c359ca1f7b1e9bbb2d95992e33bc92394f8f22254d340daa2769c0a2494";
        public const string MaizeLdsLogo = "0x2fa975f47dc5929980a8bc01ad5173302a9f6f246ae219ac7a0a4592547cdf87";
        public static string BaseDirectory = (AppDomain.CurrentDomain.BaseDirectory).Replace("\\", "/");
        public const string InputFolder = "Input/";
        public const string OutputFolder = "Output/";
        public const string EnvironmentPath = "Input/Environment/";
        public const string InputFile = "Input.txt";
        public const string BanishFile = "Banish.txt";
        public const decimal LcrTransactionFee = 0.002m;
        public static Environment GetNetworkConfig(int variable)
        {
            if (variable == 1)
            {
                return new Environment
                {
                    Url = "https://api3.loopring.io/",
                    Exchange = "0x0BABA1Ad5bE3a5C0a66E7ac838a129Bf948f1eA4",
                    NftFactory = "0xc852aC7aAe4b0f0a0Deb9e8A391ebA2047d80026",
                    NftFactoryCollection = "0x97BE94250AEF1Df307749aFAeD27f9bc8aB911db",
                    MyAccountId = 79142,
                    MyAccountAddress = "0x37EA02537f3A7A7fFC221125245905Be3D5423e6",
                };
            }
            else if (variable == 5)
            {
                return new Environment
                {
                    Url = "https://uat2.loopring.io/",
                    Exchange = "0x12b7cccF30ba360e5041C6Ce239C9a188B709b2B",
                    NftFactory = "0x0ad87482a1bfd0B3036Bb4b13708C88ACAe1b8bA",
                    NftFactoryCollection = "0x0ad87482a1bfd0B3036Bb4b13708C88ACAe1b8bA",
                    MyAccountId = 15504,
                    MyAccountAddress = "0x37EA02537f3A7A7fFC221125245905Be3D5423e6",
                };
            }
            else
            {
                throw new ArgumentException("Invalid variable value.");
            }
        }
        public class Environment
        {
            public string? Url { get; set; }
            public string? Exchange { get; set; }
            public string? NftFactory { get; set; }
            public string? NftFactoryCollection { get; set; }
            public int MyAccountId { get; set; }
            public string? MyAccountAddress { get; set; }
        }
    }
}

