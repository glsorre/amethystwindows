using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AmethystWindows.Hotkeys
{
    public partial class HotkeyEditorControl
    {
        public static readonly DependencyProperty ViewModelHotkeyProperty =
            DependencyProperty.Register(nameof(ViewModelHotkey), typeof(ViewModelHotkey),
                typeof(HotkeyEditorControl),
                new FrameworkPropertyMetadata(default(ViewModelHotkey),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ViewModelHotkey ViewModelHotkey
        {
            get => (ViewModelHotkey)GetValue(ViewModelHotkeyProperty);
            set => SetValue(ViewModelHotkeyProperty, value);
        }

        public string Description
        {
            get;
            set;
        }

        public HotkeyEditorControl()
        {
            InitializeComponent();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Don't let the event pass further
            // because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None &&
                (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                ViewModelHotkey = new ViewModelHotkey(Description, null);
                return;
            }

            // If no actual key was pressed - return
            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps)
            {
                return;
            }

            // Update the value
            ViewModelHotkey = new ViewModelHotkey(Description, new Hotkey(key, modifiers));
        }
    }
}
