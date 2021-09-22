using AmongUsHack.Exts;
using Memory;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AmongUsHack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mem mem = new Mem();
        int PID;
        string MA_IMPOSTER = "GameAssembly.dll+01DAEE64,2C,48,40,5C,0,34,2C";
        string MA_SPEED = "GameAssembly.dll+01DA93BC,5C,24,14";
        string MA_KILL_COOLDOWN = "GameAssembly.dll+01D72588,5C,0,8,18,10,C,44";
        string MA_CREWMATE_VISION = "GameAssembly.dll+00610A08,15C,188,20,50,0,1A4,18";
        string MA_IMPOSTER_VISION = "GameAssembly.dll+01DA93BC,4C,184,C,5C,24,1C";
        bool isImposter = false;
        bool noKillCoolDownEnabled = false;
        bool maxVisionEnabled = false;
        public MainWindow()
        {
            InitializeComponent();
            HotkeysManager.SetupSystemHook();

            Closing += App_Closing;
        }

        public Task ValueCheck()
        {
            while (true)
            {
                if (PID <= 0) continue;
                #region Continous Memory Checks
                #region Kill Cooldown
                if(noKillCoolDownEnabled)
                    _ = mem.WriteMemory(MA_KILL_COOLDOWN, "float", "0");
                if (maxVisionEnabled)
                {
                    if (isImposter)
                        _ = mem.WriteMemory(MA_IMPOSTER_VISION, "float", "5");
                    else
                        _ = mem.WriteMemory(MA_CREWMATE_VISION, "float", "5");
                }
                #endregion
                #endregion
                isImposter = mem.ReadInt(MA_IMPOSTER) == 1;
                float currSpeed = mem.ReadFloat(MA_SPEED, round: false);

                Action invoking = new Action(() =>
                {
                    if (isImposter)
                        isImposter_lbl.Foreground = new SolidColorBrush(Color.FromRgb(55, 191, 94));
                    else
                        isImposter_lbl.Foreground = new SolidColorBrush(Color.FromRgb(224, 83, 83));
                    speed_lbl.Content = $"Speed: {currSpeed}";
                });
                isImposter_lbl.Dispatcher.Invoke(invoking);
                speed_lbl.Dispatcher.Invoke(invoking);
                Thread.Sleep(250);
            }

        }

        public void ImposterToggle()
        {
            isImposter = !isImposter;
            string value = isImposter ? "1" : "0";
            _ = mem.WriteMemory(MA_IMPOSTER, "int", value);
        }
        public void SpeedChange()
        {
            SetValue setValue = new SetValue();
            setValue.ShowDialog();
            if(setValue.formButtons == AmongUsHack.SetValue.FormButtons.Ok)
            {
                _ = mem.WriteMemory(MA_SPEED, "float", setValue.value);
            }
        }

        private void App_Closing(object sender, CancelEventArgs e)
        {
            HotkeysManager.ShutdownSystemHook();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            PID = mem.GetProcIdFromName("Among Us");
            if (PID <= 0) return;
            GlobalHotkeys timphk = new GlobalHotkeys(ModifierKeys.None, Key.X, new Action(async () => { await Task.Run(ImposterToggle); }));
            GlobalHotkeys cshk = new GlobalHotkeys(ModifierKeys.Control, Key.C, SpeedChange);
            HotkeysManager.AddHotkey(timphk);
            HotkeysManager.AddHotkey(cshk);
            mem.OpenProcess(PID);
            MessageBox.Show($"Found process! {PID}");
            try
            {
                await Task.Run(() => ValueCheck());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void noKillCooldown_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            noKillCoolDownEnabled = true;
        }

        private void noKillCooldown_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            noKillCoolDownEnabled = false;
        }

        private void maxVision_checkbox_Checked(object sender, RoutedEventArgs e)
        {
            maxVisionEnabled = true;
        }

        private void maxVision_checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            maxVisionEnabled = true;
        }
    }
}
