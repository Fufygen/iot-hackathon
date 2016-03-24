// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Hand.Core;
using Shared;
using System.Threading;
using System.Xml.Serialization;
using Hand.Commands;

namespace Hand
{
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += MainPage_Loaded;

            CommandsList.ItemsSource = Commands;
            //Messages.ItemsSource = _messages;
            KeyboardControl = true;
            Record = true;
            //IndividualPinsNumbers.ItemsSource = _pinNumbers;

            //_pins = new PinSource();
            //InitPins();
        }

        private MotorsController _controller;

        
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _controller = new MotorsController();
        }


        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Motor motor = (Motor)button.DataContext; 
            motor.Stop();
            WriteMessage("Stop");
        }

        private void Button_Click_On(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext != null)
            {
                Switch switchPin = (Switch)button.DataContext;
                switchPin.On();
                
                WriteMessage($"Pin {switchPin.Number} set on High");
            }
        }

        
        private void Button_Click_Off(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.DataContext != null)
            {
                Switch switchPin = (Switch)button.DataContext;
                switchPin.Off();
                WriteMessage($"Pin {switchPin.Number} set on Low");
            }
        }

        private void WriteMessage(string message)
        {
            _messages.Insert(0, DateTime.Now.ToString("u") + ": " + message);
        }


        public bool KeyboardControl
        {
            get { return (bool)GetValue(KeyboardControlProperty); }
            set { SetValue(KeyboardControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for KeyboardControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyboardControlProperty =
            DependencyProperty.Register("KeyboardControl", typeof(bool), typeof(MainPage), new PropertyMetadata(false));



        private void MoveMotorForward(IMotor motor)
        {
            if (motor.IsMoving == false)
            {
                if (Record)
                {
                    _recorder.Start(motor, Direction.Forward);
                }
                motor.Forward();
            }
        }

        private void MoveMotorBackward(IMotor motor)
        { 
            if (motor.IsMoving == false)
            {
                if (Record)
                {
                    _recorder.Start(motor, Direction.Backward);
                }
                motor.Backward();
            }
        }

        private void Page_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)  
         {
            Debug.WriteLine(e.Key.ToString());

            if (KeyboardControl)
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.NumberPad1:
                        MoveMotorForward(_controller.Motor1);
                        break;
                    case Windows.System.VirtualKey.NumberPad3:
                        MoveMotorBackward(_controller.Motor1);
                        break;
                    case Windows.System.VirtualKey.NumberPad7:
                        MoveMotorForward(_controller.Motor2);
                        break;
                    case Windows.System.VirtualKey.NumberPad4:
                        MoveMotorBackward(_controller.Motor2);
                        break;
                    case Windows.System.VirtualKey.NumberPad8:
                        MoveMotorForward(_controller.Motor3);
                        break;
                    case Windows.System.VirtualKey.NumberPad5:
                        MoveMotorBackward(_controller.Motor3);
                        break;
                    case Windows.System.VirtualKey.NumberPad9:
                        MoveMotorForward(_controller.Motor4);
                        break;
                    case Windows.System.VirtualKey.NumberPad6:
                        MoveMotorBackward(_controller.Motor4);
                        break;
                    case Windows.System.VirtualKey.O:
                        MoveMotorForward(_controller.Hand);
                        break;
                    case Windows.System.VirtualKey.P:
                        MoveMotorBackward(_controller.Hand);
                        break;
                    case Windows.System.VirtualKey.Escape:
                        Cancel();

                        Application.Current.Exit();
                        break;
                }

                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }

        }

        private void Page_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyboardControl)
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.NumberPad1:
                    case Windows.System.VirtualKey.NumberPad3:
                        StopMotor(_controller.Motor1);
                        break;
                    case Windows.System.VirtualKey.NumberPad7:
                    case Windows.System.VirtualKey.NumberPad4:
                        StopMotor(_controller.Motor2);
                        break;
                    case Windows.System.VirtualKey.NumberPad8:
                    case Windows.System.VirtualKey.NumberPad5:
                        StopMotor(_controller.Motor3);
                        break;
                    case Windows.System.VirtualKey.NumberPad9:
                    case Windows.System.VirtualKey.NumberPad6:
                        StopMotor(_controller.Motor4);
                        break;
                    case Windows.System.VirtualKey.O:
                    case Windows.System.VirtualKey.P:
                        StopMotor(_controller.Hand);
                        break;
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void StopMotor(IMotor motor)
        {
            motor.Stop();
            if (Record)
            {
                var command = _recorder.Stop();

                if (command != null)
                {
                    Commands.Add(command);
                }
            }
        }

        private CommandsRecorder _recorder = new CommandsRecorder();

        private ObservableCollection<MotorCommand> Commands = new ObservableCollection<MotorCommand>();

        private void Cancel()
        {
            if (_cancel != null && _cancel.IsCancellationRequested == false)
            {
                _cancel.Cancel();
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs args)
        {
            Cancel();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs args)
        {
            var item = CommandsList.SelectedItem as MotorCommand;
            if (item != null)
            {
                Commands.Remove(item);
            }
        }
        private CommandsSource _source = new CommandsSource();

        private async void Button_SaveCommands_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                await _source.SaveAsync(Commands);
            }
            catch { }
        }

        private async void Button_LoadCommands_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                var cmds = await _source.LoadAsync();
                if (cmds != null)
                {
                    Commands.Clear();
                    foreach (var cmd in cmds)
                    {
                        Commands.Add(cmd);
                    }
                }
            }
            catch
            {

            }
        }

        private CancellationTokenSource _cancel;



        public bool Record
        {
            get { return (bool)GetValue(RecordProperty); }
            set { SetValue(RecordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Record.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RecordProperty =
            DependencyProperty.Register("Record", typeof(bool), typeof(MainPage), new PropertyMetadata(true));



        private async void Button_PlayCommands_Click(object sender, RoutedEventArgs e)
        {
            var commands = Commands.ToArray();
            if (commands.Any())
            {
                Record = false;
                using (_cancel = new CancellationTokenSource())
                {
                    await RunCommandsAsync(commands, _cancel.Token);
                }
                _cancel = null;
                Record = true;
            }
        }

        private async Task RunCommandsAsync(IEnumerable<MotorCommand> commands, CancellationToken token)
        {
            IMotor lastMotor = null;

            foreach (var cmd in commands)
            {
                if (token.IsCancellationRequested)
                {
                    if (lastMotor != null)
                    {
                        StopMotor(lastMotor);
                    }
                    return;
                }

                var motor = _controller.Motors.FirstOrDefault(m => m.Id == cmd.MotorId);
                if (motor != null)
                {
                    if (cmd.Direction == Direction.Forward)
                    {
                        lastMotor = motor;
                        MoveMotorForward(motor);
                    }
                    else if (cmd.Direction == Direction.Backward)
                    {
                        lastMotor = motor;
                        MoveMotorBackward(motor);
                    }
                    else
                    {
                        continue;
                    }
                    await Task.Delay(cmd.Duration);
                    StopMotor(motor);
                }
            }
        }

        private void CommandsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = CommandsList.SelectedItem != null;
        }

        private void Button_ClearCommands_Click(object sender, RoutedEventArgs e)
        {
            Commands.Clear();
        }

        private void CheckBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            e.Handled = false;
        }

        private void CheckBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            e.Handled = false;

        }
    }

   

}
