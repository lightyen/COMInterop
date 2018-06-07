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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.TextServices;
namespace WpfApp1 {

    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            if (Resources[nameof(InputMethodCollection)] is InputMethodCollection collec) {
                InputProcessorProfiles inputProcessorProfiles = new InputProcessorProfiles();

                //var kk = inputProcessorProfiles.GetInputProcessorInfo();

                foreach (var id in inputProcessorProfiles.LanguageIDs) {
                    var profiles = inputProcessorProfiles.GetLanguageProfiles(id);
                    string langName = inputProcessorProfiles.GetLanguageName(id);
                    foreach (var profile in profiles) {
                        
                        if (inputProcessorProfiles.IsEnabledLanguageProfile(profile)) {
                            collec.Add(new InputMethod() {
                                Profile = profile,
                                Description = $"{langName} - {inputProcessorProfiles.GetLanguageProfileDescription(profile)}"
                            });
                        }
                    }
                }
            }
        }

        private void SelectInputMethod_Click(object sender, RoutedEventArgs e) {
            int i = InputMethodListBox.SelectedIndex;
            if (i >= 0) {
                var im = InputMethodListBox.SelectedValue as InputMethod;
                using (InputProcessorProfiles inputProcessorProfiles = new InputProcessorProfiles()) {
                    try {
                        inputProcessorProfiles.ActivateLanguageProfile(im.Profile);
                    } catch (Exception) {

                    }
                    
                }
            }
        }

        private void CurrentInputMethod_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button) {
                using (InputProcessorProfiles inputProcessorProfiles = new InputProcessorProfiles()) {
                    button.Content = inputProcessorProfiles.GetCurrentLanguageProfileName();
                }
            }
        }

        private void SetDefaultInputMethod_Click(object sender, RoutedEventArgs e) {
            if (sender is Button button) {

                int i = InputMethodListBox.SelectedIndex;
                if (i >= 0) {
                    var im = InputMethodListBox.SelectedValue as InputMethod;
                    using (InputProcessorProfiles inputProcessorProfiles = new InputProcessorProfiles()) {
                        try {
                            inputProcessorProfiles.SetDefaultLanguageProfile(im.Profile);
                            button.Content = "設定預設輸入法";
                        } catch (Exception ex) {
                            button.Content = ex.Message;
                        }
                    }
                }
            }
        }
    }

    public class InputMethod {
        public LanguageProfile Profile { get; set; }
        public string Description { get; set; }
    }

    public class InputMethodCollection : System.Collections.ObjectModel.ObservableCollection<InputMethod> {

    }
}
