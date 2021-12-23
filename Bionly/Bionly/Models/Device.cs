﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Bionly.Enums.Connection;

namespace Bionly.Models
{
    public class Device : INotifyPropertyChanged
    {
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + "/Devices/";

        private string _id;
        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private string _ipAdress;
        public string IpAddress
        {
            get => _ipAdress;
            set
            {
                if (_ipAdress != value)
                {
                    _ipAdress = value;
                    OnPropertyChanged(nameof(IpAddress));
                }
            }
        }

        private ushort _camPort;
        public ushort CameraPort
        {
            get => _camPort;
            set
            {
                if (_camPort != value)
                {
                    _camPort = value;
                    OnPropertyChanged(nameof(CameraPort));
                }
            }
        }

        private ushort _ftpPort;
        public ushort FtpPort
        {
            get => _ftpPort;
            set
            {
                if (_ftpPort != value)
                {
                    _ftpPort = value;
                    OnPropertyChanged(nameof(FtpPort));
                }
            }
        }

        private bool[] _settings = new bool[2];
        public bool[] Settings
        {
            get => _settings;
            set
            {
                if (_settings != value)
                {
                    _settings = value;
                    OnPropertyChanged(nameof(Settings));
                }
            }
        }

        private float _currentTemp = 0;
        [JsonIgnore]
        public float CurrentTemp
        {
            get => _currentTemp;
            set
            {
                if (_currentTemp != value)
                {
                    _currentTemp = value;
                    OnPropertyChanged(nameof(CurrentTemp));
                }
            }
        }

        private float _currentHumi = 0;
        [JsonIgnore]
        public float CurrentHumi
        {
            get => _currentHumi;
            set
            {
                if (_currentHumi != value)
                {
                    _currentHumi = value;
                    OnPropertyChanged(nameof(CurrentHumi));
                }
            }
        }

        private ConnectionStatus _connected = ConnectionStatus.Error;
        [JsonIgnore]
        public ConnectionStatus Connected
        {
            get => _connected;
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    OnPropertyChanged(nameof(Connected));
                }
            }
        }

        /// <returns>
        /// Returns the matching image of the connection state.
        /// </returns>
        [JsonIgnore]
        public ImageSource DeviceSymbol
        {
            get
            {
                return Connected switch
                {
                    ConnectionStatus.Connected => ImageSource.FromFile("Resources/icon_cloud_done.png"),
                    ConnectionStatus.Connecting => ImageSource.FromFile("Resources/icon_cloud_sync.png"),
                    ConnectionStatus.Disconnected => ImageSource.FromFile("Resources/icon_cloud_unavailable.png"),
                    ConnectionStatus.Error => ImageSource.FromFile("Resources/icon_cloud_cross.png"),
                    _ => null,
                };
            }
        }

        public Device() { }

        public async Task<bool> Delete()
        {
            try
            {
                File.Delete(path + $"{Id}.json");
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu löschen.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return false;
        }
        public async Task<bool> Save()
        {
            try
            {
                if (string.IsNullOrEmpty(Id)) NewId();
                _ = Directory.CreateDirectory(path);
                File.WriteAllText(path + $"{Id}.json", JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu speichern.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return false;
        }
        public static async Task<Device> Load(string file)
        {
            try
            {
                _ = Directory.CreateDirectory(path);
                return JsonConvert.DeserializeObject<Device>(File.ReadAllText(file));
            }
            catch (UnauthorizedAccessException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Die App besitzt nicht die benötigten Berechtigungen um dieses Gerät zu laden.", "OK");
            }
            catch (FileNotFoundException)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", "Dieses Gerät wurde nicht gefunden.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fehler", ex.Message, "OK");
            }
            return new();
        }

        public bool CheckAdress(string input)
        {
            if (IPAddress.TryParse(input, out IPAddress address))
            {
                IpAddress = address.ToString();
                return true;
            }
            return false;
        }
        public IPAddress GetAdress()
        {
            if (IPAddress.TryParse(IpAddress, out IPAddress adress))
            {
                return adress;
            }
            else
            {
                return null;
            }
        }
        public string NewId()
        {
            byte[] codebytes = new byte[8];
            string code;
            do
            {
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(codebytes);
                }
                code = BitConverter.ToString(codebytes).ToLower().Replace("-", "");
            } while (File.Exists(path + $"{code}.json"));
            Id = code;
            return Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
