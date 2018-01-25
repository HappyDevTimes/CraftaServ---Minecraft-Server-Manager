using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Mono.Nat;
using System.Threading;
using System.Windows.Media;
using System.Windows.Documents;
using PortforwardWPF.Windows;
using PortforwardWPF.Classes;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using CraftaServ.Windows;
using System.Media;

namespace PortforwardWPF
{

    //LICENSE: MIT
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //RECOLOR THE ERROR MESSAGES
    //CLOSE THE JAVA AFTER THE APPLICATION CLOSES
    //ADD A STOP SERVER BUTTON
    //ADD A COMMAND BUTTON
    //ADD OP FEATURE


        //Error When Generating EULA, FIX!
        //ADD A RED MESSAGE THAT SAYS IF THE PORT IS BOUND, CHECK FOR WARM] FAILED TO BIND PORT 
        //FIX SPACING INBETWEEN THE BUTTONS
        //FIX THE (IF SAY CHECKBOX IS CHECKED) add /say infront of message
        //ALLOW ENTER KEY AS A SEND FUNCTION
        //REDIRECT THE PLUGIN DOWNLOAD TO THE PLUGINS FOLDER -- IF PLUGINFOLDER DOESNT EXIST CREATE ONE -- 

    //Quick start feature
    //Save application settings in manifest file


    //MAKE SURE WHEN USER LEAVES YOU REMOVE THE USER FROM THE LISTVIEW ASWELL AS REFRESH THE LISTVIEW UI.
    //DEAL WITH THE LOGIN / LOGOUT BUG [< + USERNAME]


    //SCRAPE THE ENTIRE LIST OF ITEMS ON MINECRAFTDB AND BIND EACH ID TO AN ITEM http://minecraft-ids.grahamedgecombe.com/
    //GIVE USER X AND SELECT ITEM FROM LIST
    public partial class MainWindow : Window
    {
        public static MainWindow instance;


        const string pattern = @"(.*): (.*)\[(.*)] logged in with entity id (.*) at ((.*))";
        const string leavePattern = @"((.*)) (.*) left the game";
        string jarName = "";
        List<int> comboItems = new List<int>() { 512, 1024, 2048, 3072, 4096, 5120, 6144, 7162, 8192, 9216, 10240 };
        public ObservableCollection<Player> thePlayers = new ObservableCollection<Player>();
        public Process process = new Process();

        public static bool PlaySoundOnLogin = false;

        private bool btnOpenWasClicked = false;

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }

        [STAThread]
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JAR Files | *.jar";
            if (ofd.ShowDialog() == true)
            {
                AppendText(rtbConsole, "[" + DateTime.Now.ToLongTimeString() + " INFO]" + ": Added the JAR file.", "Green" + Environment.NewLine);
                rtbConsole.ScrollToEnd();
                jarName = Path.GetFileName(ofd.FileName);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btnOpenPort_Click(object sender, RoutedEventArgs e)
        {
            btnOpenWasClicked = true;
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();
        }

        #region NATSettings
        private void DeviceFound(object sender, DeviceEventArgs args)
        {
            if (btnOpenWasClicked == true)
            {
                INatDevice device = args.Device;
                Mapping minecraftTCP = new Mapping(Protocol.Tcp, 25565, 25565);
                Mapping minecraftUDP = new Mapping(Protocol.Udp, 25565, 25565);
                minecraftTCP.Description = "MinecraftTCP";
                minecraftUDP.Description = "MinecraftUDP";
                device.CreatePortMap(minecraftTCP);
                device.CreatePortMap(minecraftUDP);


                foreach (Mapping portMap in device.GetAllMappings())
                {
                    Debug.Print(portMap.ToString());
                }

                MessageBox.Show("Port 25565 has been opened.");
                MessageBoxResult diag = MessageBox.Show("This is the IP you will give to your friends: " + device.GetExternalIP().ToString() + ":25565" + " Do you wanna copy the IP? ",
                    "Success", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (diag == MessageBoxResult.Yes)
                {
                    Thread thread = new Thread(() => Clipboard.SetText(device.GetExternalIP() + ":25565"));
                    thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                    thread.Start();
                    thread.Join(); //Wait for the thread to end
                }
            }
        }
        #endregion

        private void btnCommand_Click(object sender, RoutedEventArgs e)
        {
            Commands Commands = new Commands();

            CommandWindow cmd = new CommandWindow();
            cmd.Show();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //IF PROCESS ISNT RUNNING HANDLE IT CORRECTLY
            process.Kill();
            System.Windows.Application.Current.Shutdown();
        }

        private void btnMinizmize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

            if (jarName != "" && tbManualRAM.Text != "")
            {
                //Create validation text could be anothing.
                var psi = new ProcessStartInfo("java", $"-Xms{tbManualRAM.Text}M -jar {jarName} -o true ");

                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardError = true;
                //Might not want to hide this incase of some weird debug shit
                psi.CreateNoWindow = true;

                process.StartInfo = psi;
                process.OutputDataReceived += Process_OutputDataReceived;
                process.ErrorDataReceived += Process_ErrorDataReceived;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            else
            {
                AppendText(rtbConsole, "[" + DateTime.Now.ToLongTimeString() + " INFO]: ERROR: Please select a .JAR file. & select the amount of ram you want the server to allocate.", "Red" + Environment.NewLine);
                rtbConsole.ScrollToEnd();

            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {

            //System.ArgumentNullException: 'Value cannot be null.'
            this.Dispatcher.Invoke(() =>
            {
                AppendText(rtbConsole, e.Data, "Red" + Environment.NewLine);
                rtbConsole.ScrollToEnd();

                if (CraftaServ.Properties.Settings.Default.PlayErrorSound == true)
                    SystemSounds.Exclamation.Play();
            });
        }

        //System.ArgumentNullException: 'Value cannot be null.'
        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                AppendText(rtbConsole, e.Data, "Green" + Environment.NewLine);
                foreach (Match ma in Regex.Matches(e.Data, pattern))
                {
                    if (!string.IsNullOrEmpty(ma.ToString()))
                    {
                        if (!ma.Groups[2].ToString().Contains("<"))
                        {
                            thePlayers.Add(new Player() { Username = ma.Groups[2].ToString() });
                            lvUsers.ItemsSource = thePlayers;
                            lvUsers.Items.Refresh();
                        }
                    }

                    if (CraftaServ.Properties.Settings.Default.PlayLoginSound == true)
                        SystemSounds.Beep.Play();
                }

                foreach (Match mat in Regex.Matches(e.Data, leavePattern))
                {
                    if (!string.IsNullOrEmpty(mat.ToString()))
                    {
                        if (!mat.Groups[2].ToString().Contains("<"))
                        {
                            var playerToRemove = thePlayers.FirstOrDefault(x => x.Username == mat.Groups[3].ToString());
                            if (playerToRemove != null)
                                thePlayers.Remove(playerToRemove);
                        }
                    }

                    if (CraftaServ.Properties.Settings.Default.PlayerDisconnectSound == true)
                        SystemSounds.Beep.Play();
                }
                rtbConsole.ScrollToEnd();
            });
        }

        public void AppendText(RichTextBox box, string text, string color)
        {
            BrushConverter bc = new BrushConverter();
            TextRange tr = new TextRange(box.Document.ContentEnd, box.Document.ContentEnd);

            tr.Text = text + Environment.NewLine;
            try
            {
                tr.ApplyPropertyValue(TextElement.ForegroundProperty,
                    bc.ConvertFromString(color));
            }
            catch (FormatException) { }
        }

        private void PART_ContentHost_GotFocus(object sender, RoutedEventArgs e)
        {
            if (tbManualRAM.Text == "Enter the amount of ram")
                tbManualRAM.Text = "";

        }

        private void PART_ContentHost_LostFocus(object sender, RoutedEventArgs e)
        {
            if (tbManualRAM.Text == "")
                tbManualRAM.Text = "Enter the amount of ram";
        }

        private void cbRam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbManualRAM.Text = comboItems[cbRam.SelectedIndex].ToString();

        }

        private void btnAutoDetect_Click(object sender, RoutedEventArgs e)
        {
            cbRam.SelectedIndex = 0;
            long memKb;
            GetPhysicallyInstalledSystemMemory(out memKb);
            tbManualRAM.Text = (memKb / 1024 / 2).ToString();
            AppendText(rtbConsole, "[" + DateTime.Now.ToLongTimeString() + " INFO]: " + (memKb / 1024) + " MB of RAM installed.. We recommend to use half.", "Green");
            rtbConsole.ScrollToEnd();
        }
        #region PlayerActions

        private void btnSendCommand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                rtbConsole.AppendText(tbToConsole.Text + Environment.NewLine);
                StreamWriter sw = process.StandardInput;
                sw.WriteLine(tbToConsole.Text);
                tbToConsole.Clear();
            }
            catch { }
        }

        private void cmndOP_Click(object sender, RoutedEventArgs e)
        {
            Commands cmd = new Commands();
            cmd.OpUser();
        }

        private void cmndsTeleport_Click(object sender, RoutedEventArgs e)
        {
            TeleportWindow tw = new TeleportWindow();
            tw.Show();
        }

        private void cmndsBan_Click(object sender, RoutedEventArgs e)
        {
            Commands cmd = new Commands();
            Player currentUser = ((Player)lvUsers.SelectedItem);
            cmd.BanUser(currentUser.Username);
        }

        private void cmndKick_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmndsGiveItem_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region Classes
        public class Player
        {
            public string Username { get; set; }
        }
        #endregion

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow ow = new OptionsWindow();
            ow.Show();
        }
    }
}
