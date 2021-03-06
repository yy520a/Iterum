﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MouseKeyboardLibrary;
using System.Threading;
using System.Speech.Recognition;
using System.IO;
using System.Xml.Serialization;

namespace GlobalMacroRecorder
{
    public partial class MacroForm : Form
    {
        #region ATTRIBUTES
        /****************** ATTRIBUTES ******************/
        private List<List<MacroEvent>> m_listevents;
        private List<MacroEvent> m_events;
        private int m_lastTimeRecorded;
        private int m_numberID;
        private bool m_speechRecognitionActivate;
        private List<System.Windows.Forms.RadioButton> m_listOfRadioButton;
        private ASR m_ASR;
        private List<List<MacroEvent>> m_listEventsWithSetting;
        private List<MacroEvent> m_eventsWithSetting;

        private MouseHook m_mouseHook;
        private KeyboardHook m_keyboardHook;

        private List<List<MacroEventSerializable>> m_listeventsSerializable;
        private List<MacroEventSerializable> m_eventsSerializable;
        private List<int> m_listOfEventsChosenToExport;
        private List<int> m_listOfEventsChosenToSetting;
        private bool m_exportEventsConfiration;
        private bool m_settingEventsConfiration;
        #endregion


        #region CONSTRUCTOR
        /****************** CONSTRUCTOR ******************/
        public MacroForm()
        {
            InitializeComponent();//Initialize the Form

            #region Initialize attributes

            #region Initialize primitive attributes
            //Initialize attributes
            m_listevents = new List<List<MacroEvent>>();
            m_events = new List<MacroEvent>();
            m_lastTimeRecorded = 0;
            m_numberID = 0;
            m_speechRecognitionActivate = false;
            m_listOfRadioButton = new List<System.Windows.Forms.RadioButton>();
            m_listEventsWithSetting = new List<List<MacroEvent>>();
            m_eventsWithSetting = new List<MacroEvent>();

            m_listeventsSerializable = new List<List<MacroEventSerializable>>();
            m_eventsSerializable = new List<MacroEventSerializable>();
            m_listOfEventsChosenToExport = new List<int>();
            m_listOfEventsChosenToSetting = new List<int>();
            m_exportEventsConfiration = false;
            m_settingEventsConfiration = false;
            #endregion

            #region Initialize member objects attributes
            //Initialize member objects attributes
            m_mouseHook = new MouseHook();
            m_keyboardHook = new KeyboardHook();
            m_ASR = new ASR(this);
            #endregion

            #endregion

            #region Initialize listener to record mouse action
            //Listener to record mouse action
            m_mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);//Record the movement of mouse
            m_mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);//Record the clic down of mouse (press)
            m_mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);//Record the clic up of mouse (release)
            #endregion

            #region Initialize listener to record keyboard action
            //Listener to record keyboard action
            m_keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);//Record the keyboard key down
            m_keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);//Record the keyboard key up
            #endregion

            #region Disable unused button
            recordStopButton.Enabled = false;//Disable the button to stop the record
            playBackMacroButton.Enabled = false;//Disable the button to playBack the record
            ExportSeveralEventsButton.Enabled = false;//Disable the button to export records
            EventSettingButton.Enabled = false;//Disable the button to setting speed of a record
            #endregion
        }
        #endregion


        #region GETTER
        /****************** GETTER ******************/
        //Get the attribute m_listOfRadioButton
        public List<System.Windows.Forms.RadioButton> getm_listOfRadioButton()
        {
            return m_listOfRadioButton;
        }

        //Get the attribute m_listOfEventsChosenToExport
        public List<int> getm_listOfEventsChosenToExport()
        {
            return m_listOfEventsChosenToExport;
        }

        //Get the attribute m_listOfEventsChosenToSetting
        public List<int> getm_listOfEventsChosenToSetting()
        {
            return m_listOfEventsChosenToSetting;
        }

        //Get the RadioButton by its id
        public System.Windows.Forms.RadioButton getRadioButtonByEventId(int idOfEvent)
        {
            #region try to get the radio button of event idOfEvent
            //try to get the radio button of event idOfEvent
            try
            {
                return m_listOfRadioButton[idOfEvent--];
            }
            //Otherwise, try to get the radio button of event 1
            catch
            {
                try
                {
                    return m_listOfRadioButton[1];
                }
                //Otherwise, return null
                catch
                {
                    return null;
                }
            }
            #endregion
        }
        #endregion

        #region SETTER
        /****************** SETTER ******************/
        //Set the attribute m_listOfEventsChosenToExport
        public void setm_listOfEventsChosenToExport(int idOfEventsChosenToExport)
        {
            m_listOfEventsChosenToExport.Add(idOfEventsChosenToExport);
        }

        //Set the attribute m_listOfEventsChosenToSetting
        public void setm_listOfEventsChosenToSetting(int idOfEventsChosenToSetting)
        {
            m_listOfEventsChosenToSetting.Add(idOfEventsChosenToSetting);
        }


        #region Setter speed of events

        //Set the speed of m_listEventsWithSetting list with the normal speed store in m_listevents
        public void setm_listEventsWithSettingWithNormalSpeed()
        {
            foreach(int currentEvent in m_listOfEventsChosenToSetting)
            {
                for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                {
                    int currentSpeedTime = m_listevents[currentEvent][i].getTimeSinceLastEvent();
                    m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                    m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                }
            }
        }

        //Set the speed of m_listEventsWithSetting list with a custom multiplier speed. Indeed, we multiply the time speed by timeSpeedMultiplier if multiplierOperand = true and we divide the time speed by timeSpeedMultiplier if multiplierOperand = false.
        public int setm_listEventsWithSettingWithCustomMultiplierSpeed(bool multiplierOperand, int timeSpeedMultiplier)
        {
            //if the user try to divide by 0.
            if (timeSpeedMultiplier == 0 && multiplierOperand == false)
            {
                #region We cannot divide the timespeed by 0.
                // Configure the message box to be displayed
                string messageBoxText = "We cannot divide by time speed by 0! We set for you the speed multiplier to 1 for the current event.";
                string caption = "Error";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);//Show message box error to inform user that the speed of an event connot be faster than instantaneous
                #endregion
                return -1;
            }
            if (timeSpeedMultiplier < 0)
            {
                #region If timeSpeedMultiplier<0, we set timeSpeedMultiplier=1 and we create a error message box to inform the user.
                // Configure the message box to be displayed
                string messageBoxText = "We cannot set the timeSpeedMultiplier<0. We set for you the speed multiplier to the absolute value of your entry for the current event. So the speed multiplier is set to "+ Math.Abs(timeSpeedMultiplier);
                string caption = "Warning";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);//Show message box warning to inform user that the speed of an event connot be faster than instantaneous
                #endregion
                if (multiplierOperand == true)
                {
                    foreach (int currentEvent in m_listOfEventsChosenToSetting)
                    {
                        for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                        {
                            int currentSpeedTime = m_listevents[currentEvent][i].getTimeSinceLastEvent() * Math.Abs(timeSpeedMultiplier);
                            m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                            m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                        }
                    }
                }
                return 0;
            }
            else
            {
                //If multiplierOperand is equal to true, we set the operand by a multiply operand
                if (multiplierOperand)
                {
                    foreach (int currentEvent in m_listOfEventsChosenToSetting)
                    {
                        for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                        {
                            int currentSpeedTime = m_listevents[currentEvent][i].getTimeSinceLastEvent() * timeSpeedMultiplier;
                            m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                            m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                        }
                    }
                }
                //If multiplierOperand is equal to true, we set the operand by a divide operand
                else
                {
                    foreach (int currentEvent in m_listOfEventsChosenToSetting)
                    {
                        for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                        {
                            int currentSpeedTime = m_listevents[currentEvent][i].getTimeSinceLastEvent() / timeSpeedMultiplier;
                            m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                            m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(currentSpeedTime);
                        }
                    }
                }
                return 1;
            }
        }

        //Set the speed of m_listEventsWithSetting list with a custom uniform speed
        public bool setm_listEventsWithSettingWithCustomUniformSpeed(int timeSpeed)
        {
            if(timeSpeed < 0)
            {
                #region If timeSpeed<0, we set timeSpeed=0 and we create a error message box to inform the user.
                // Configure the message box to be displayed
                string messageBoxText = "The speed of an event cannot be faster than instantaneous! We set for you the speed of current event to 0.";
                string caption = "Warning";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                MessageBox.Show(messageBoxText, caption, button, icon);//Show message box warning to inform user that the speed of an event connot be faster than instantaneous
                #endregion
                foreach (int currentEvent in m_listOfEventsChosenToSetting)
                {
                    for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                    {
                        m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(0);
                        m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(0);
                    }
                }
                return false;//Return false if timeSpeed < 0
            }
            else
            {
                foreach (int currentEvent in m_listOfEventsChosenToSetting)
                {
                    for (int i = 0; i < m_listEventsWithSetting[currentEvent].Count; i++)
                    {
                        m_listEventsWithSetting[currentEvent][i].setTimeSinceLastEvent(timeSpeed);
                        m_listeventsSerializable[currentEvent][i].setTimeSinceLastEvent(timeSpeed);
                    }
                }
                return true;//Return true if timeSpeed >= 0
            }
        }

        #endregion

        //Set the attribute m_exportEventsConfiration
        public void setm_exportEventsConfiration(bool exportEventsConfiration)
        {
            m_exportEventsConfiration = exportEventsConfiration;
        }

        //Set the attribute m_settingEventsConfiration
        public void setm_settingEventsConfiration(bool settingEventsConfiration)
        {
            m_settingEventsConfiration = settingEventsConfiration;
        }
        
        #endregion


        #region METHODS
        /****************** METHODS ******************/

        #region Methods to manage the record of user actions

        //Record the movement of mouse
        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            int TimeSinceLastEvent = Environment.TickCount - m_lastTimeRecorded;
            m_events.Add(new MacroEvent(MacroEventType.MouseMove, e, TimeSinceLastEvent));
            m_eventsWithSetting.Add(new MacroEvent(MacroEventType.MouseMove, e, TimeSinceLastEvent));
            m_lastTimeRecorded = Environment.TickCount;
            MouseEventArgsSerializable eSerializable = new MouseEventArgsSerializable(e.Button, e.Clicks, e.X, e.Y, e.Delta);
            m_eventsSerializable.Add(new MacroEventSerializable(MacroEventType.MouseMove, eSerializable, TimeSinceLastEvent));
        }

        //Record the clic down of mouse (press)
        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            int TimeSinceLastEvent = Environment.TickCount - m_lastTimeRecorded;
            m_events.Add(new MacroEvent(MacroEventType.MouseDown, e, TimeSinceLastEvent));
            m_eventsWithSetting.Add(new MacroEvent(MacroEventType.MouseDown, e, TimeSinceLastEvent));
            m_lastTimeRecorded = Environment.TickCount;
            MouseEventArgsSerializable eSerializable = new MouseEventArgsSerializable(e.Button, e.Clicks, e.X, e.Y, e.Delta);
            m_eventsSerializable.Add(new MacroEventSerializable(MacroEventType.MouseDown, eSerializable, TimeSinceLastEvent));
        }

        //Record the clic up of mouse (release)
        private void mouseHook_MouseUp(object sender, MouseEventArgs e)
        {
            int TimeSinceLastEvent = Environment.TickCount - m_lastTimeRecorded;
            m_events.Add(new MacroEvent(MacroEventType.MouseUp, e, TimeSinceLastEvent));
            m_eventsWithSetting.Add(new MacroEvent(MacroEventType.MouseUp, e, TimeSinceLastEvent));
            m_lastTimeRecorded = Environment.TickCount;
            MouseEventArgsSerializable eSerializable = new MouseEventArgsSerializable(e.Button, e.Clicks, e.X, e.Y, e.Delta);
            m_eventsSerializable.Add(new MacroEventSerializable(MacroEventType.MouseUp, eSerializable, TimeSinceLastEvent));
        }


        //Record the keyboard key down
        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            int TimeSinceLastEvent = Environment.TickCount - m_lastTimeRecorded;
            m_events.Add(new MacroEvent(MacroEventType.KeyDown, e, TimeSinceLastEvent));
            m_eventsWithSetting.Add(new MacroEvent(MacroEventType.KeyDown, e, TimeSinceLastEvent));
            m_lastTimeRecorded = Environment.TickCount;
            KeysEventArgsSerializable eSerializable = new KeysEventArgsSerializable(e.KeyData);
            m_eventsSerializable.Add(new MacroEventSerializable(MacroEventType.KeyDown, eSerializable, TimeSinceLastEvent));
        }


        //Record the keyboard key up
        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            int TimeSinceLastEvent = Environment.TickCount - m_lastTimeRecorded;
            m_events.Add(new MacroEvent(MacroEventType.KeyUp, e, TimeSinceLastEvent));
            m_eventsWithSetting.Add(new MacroEvent(MacroEventType.KeyUp, e, TimeSinceLastEvent));
            m_lastTimeRecorded = Environment.TickCount;
            KeysEventArgsSerializable eSerializable = new KeysEventArgsSerializable(e.KeyData);
            m_eventsSerializable.Add(new MacroEventSerializable(MacroEventType.KeyUp, eSerializable, TimeSinceLastEvent));
        }

        #endregion


        #region Methods to manage the enable state of buttons

        //Enable used button when we Start Record
        private void enableUsedButtonWhenStartRecord()
        {
            #region Enable recordStopButton Button
            recordStopButton.Enabled = true;//Enable recordStopButton Button
            #endregion

            #region Enable ExportSeveralEventsButton Button
            ExportSeveralEventsButton.Enabled = true;//Enable the button to export records
            #endregion
        }

        //Disable unused button when we Start Record
        private void disableUnusedButtonWhenStartRecord()
        {
            #region Disable recordStartButton Button
            recordStartButton.Enabled = false;//Disable recordStartButton Button
            #endregion

            #region Disable playBackMacroButton Button
            playBackMacroButton.Enabled = false;//Disable playBackMacroButton Button
            #endregion

            #region Disable EventSettingButton Button
            EventSettingButton.Enabled = false;//Disable the button to setting speed of a record
            #endregion

            #region For each RadioButton component in ScrollPanel of FormModePersonalize Form, disable RadioButton
            //For each component in ScrollPanel of FormModePersonalize Form
            foreach (System.Windows.Forms.RadioButton o in ScrollPanel.Controls)
            {
                    o.Enabled = false;//Disable RadioButton
            }
            #endregion
        }

        //Enable used button when we Stop Record
        private void enableUsedButtonWhenStopRecord()
        {
            #region Enable recordStartButton Button
            recordStartButton.Enabled = true;//Enable recordStartButton Button
            #endregion

            #region Enable playBackMacroButton Button
            playBackMacroButton.Enabled = true;//Enable playBackMacroButton Button
            #endregion

            #region Enable EventSettingButton Button
            EventSettingButton.Enabled = true;//Enable the button to setting speed record
            #endregion

            #region Enable ExportSeveralEventsButton Button
            ExportSeveralEventsButton.Enabled = true;//Enable the button to export records
            #endregion

            #region For each RadioButton component in ScrollPanel of FormModePersonalize Form, enable RadioButton
            //For each component in ScrollPanel of FormModePersonalize Form
            foreach (System.Windows.Forms.RadioButton o in ScrollPanel.Controls)
            {
                o.Enabled = true;//Enable RadioButton
            }
            #endregion
        }

        //Disable unused button when we Stop Record
        private void disableUnusedButtonWhenStopRecord()
        {
            #region Disable recordStopButton Button
            recordStopButton.Enabled = false;//Disable recordStopButton Button
            #endregion
        }

        //Enable used button when we Playback Record Stop
        private void enableUsedButtonWhenPlaybackRecordStop()
        {
            #region Enable recordStartButton Button and update attribute m_isEnableRecordStartButton
            recordStartButton.Enabled = true;//Enable recordStartButton Button
            #endregion

            #region Enable recordStopButton Button and update attribute m_isEnableRecordStopButton
            recordStopButton.Enabled = false;//Enable recordStopButton Button
            #endregion

            #region Enable playBackMacroButton Button and update attribute m_isEnablePlayBackMacroButton
            playBackMacroButton.Enabled = true;//Enable playBackMacroButton Button
            #endregion

            #region Enable ExportSeveralEventsButton Button
            ExportSeveralEventsButton.Enabled = true;//Enable the button to export records
            #endregion

            #region Enable EventSettingButton Button
            EventSettingButton.Enabled = true;//Enable the button to setting speed record
            #endregion

            #region For each RadioButton component in ScrollPanel of FormModePersonalize Form, enable RadioButton
            //For each component in ScrollPanel of FormModePersonalize Form
            foreach (System.Windows.Forms.RadioButton o in ScrollPanel.Controls)
            {
                o.Enabled = true;//Enable RadioButton
            }
            #endregion
        }

        //Disable unused button when we Playback Record
        private void disableUnusedButtonWhenPlaybackRecord()
        {
            #region Disable recordStartButton Button
            recordStartButton.Enabled = false;//Disable recordStartButton Button
            #endregion

            #region Disable recordStopButton Button
            recordStopButton.Enabled = false;//Disable recordStopButton Button
            #endregion

            #region Disable playBackMacroButton Button
            playBackMacroButton.Enabled = false;//Disable playBackMacroButton Button
            #endregion

            #region Disable EventSettingButton Button
            EventSettingButton.Enabled = false;//Disable the button to setting speed record
            #endregion

            #region For each RadioButton component in ScrollPanel of FormModePersonalize Form, disable RadioButton
            //For each RadioButton component in ScrollPanel of FormModePersonalize Form
            foreach (System.Windows.Forms.RadioButton o in ScrollPanel.Controls)
            {
                o.Enabled = false;//Disable RadioButton
            }
            #endregion
        }

        #endregion


        #region Methods to manage the click on main buttons

        //Event when the user click on the Start button
        public void recordStartButton_Click(object sender, EventArgs e)
        {
            #region Enable/Disable used/unused button when we Start Record
            //Enable/Disable used/unused button when we Start Record
            disableUnusedButtonWhenStartRecord();//Disable unused button when we Start Record
            enableUsedButtonWhenStartRecord();//Enable used button when we Start Record
            #endregion

            #region Create new RadioButton dynamically for the event that we start to record

            #region Create new RadioButton variable, set it is name dynamically and update variable
            //Create new RadioButton dynamically for the event that we start to record
            m_numberID++;//Increment the number of event
            System.Windows.Forms.RadioButton radb = new System.Windows.Forms.RadioButton();//Create a RadioButton
            radb.Text = "Event" + m_numberID;//Set the text of RadioButton with the name Event following by the number of events
            Point lastLocationRadioButton=new System.Drawing.Point(0, 0);//Create a variable of type Point
            #endregion

            #region Uncheck all Radio button and check the last Radio button that we want to add dynamically
            //For each component in ScrollPanel of FormModePersonalize Form, we uncheck all Radio button
            foreach (object o in ScrollPanel.Controls)
            {
                //If the component is a RadioButton (i.e: If the type of component is a RadioButton).
                if (o is System.Windows.Forms.RadioButton)
                {
                    System.Windows.Forms.RadioButton currentRadioButton = (System.Windows.Forms.RadioButton)o;//Stock the component (which is a RadioButton) on a variable called currentRadioButton.
                    lastLocationRadioButton = currentRadioButton.Location;//Set the variable lastLocationRadioButton with the location of the current RadioButton.
                    //If the current RadioButton is checked
                    if (currentRadioButton.Checked == true)
                    {
                        currentRadioButton.Checked = false;//Uncheck all Radio button exept the last one.
                    }
                }
            }
            radb.Checked = true;//Check the last Radio button
            #endregion

            #region Set the position of the new Radio button
            //If it is the first record
            if (m_numberID == 1)
            {
                radb.Location = new System.Drawing.Point(10, 0);//Set the position of RadioButton in the FormModePersonalize Form
            }
            //If it is not the first record
            else
            {
                radb.Location = new System.Drawing.Point(10, lastLocationRadioButton.Y + 30);//Set the position of RadioButton in the FormModePersonalize Form
            }
            #endregion

            #region Add the new Radio button to the ScrollPanel
            ScrollPanel.Controls.Add(radb);//Add RadioButton in panel called ScrollPanel in FormModePersonalize Form
            #endregion

            #region Add the new Radio button to the list m_listOfRadioButton
            m_listOfRadioButton.Add(radb);//Add the new Radio button to the list m_listOfRadioButton
            #endregion

            #endregion

            #region Clear the list dedicated to store the action of user (MacroEvent) and start the TickCount
            m_events.Clear();
            m_eventsWithSetting.Clear();
            m_lastTimeRecorded = Environment.TickCount;
            #endregion

            #region By default there are 10 items recognize in the grammar. So, if the user create more than 10 events, we start to add dynamically the number of event in the grammar
            //By default there are 10 items recognize in the grammar. So, if the user create more than 10 events, we start to add dynamically the number of event in the grammar
            if (m_numberID > 10)
            {
                m_ASR.addItemInGrammarAndReloadEngine("numbers", 0, m_numberID.ToString());
            }
            #endregion

            #region Start the record of keyboard and mouse action
            m_keyboardHook.Start();
            m_mouseHook.Start();
            #endregion
        }


        //Event when the user click on the Stop button
        public void recordStopButton_Click(object sender, EventArgs e)
        {
            #region Enable/Disable used/unused button when we Stop Record
            disableUnusedButtonWhenStopRecord();//Disable unused button when we Stop Record
            enableUsedButtonWhenStopRecord();//Enable used button when we Stop Record
            #endregion

            #region Stop the record
            m_keyboardHook.Stop();//Stop to record the keyboard input
            m_mouseHook.Stop();//Stop to record the mouse input
            #endregion

            #region Add the last record to the list of events
            //Add the last record to the m_listevents which contains the all lists of events.
            List<MacroEvent> testEvents = new List<MacroEvent>(m_events);//Make a copy of list m_events called testEvents. So now, m_events=testEvents.
            m_listevents.Add(testEvents);//Add the list testEvents which is a copy of m_events and which contains the last record. We do a copy because otherwise, it is added by reference.
            #endregion

            #region Add the last record to the list of events with setting m_listEventsWithSetting
            List<MacroEvent> testEventsWithSetting = new List<MacroEvent>(m_eventsWithSetting);//Make a copy of list m_eventsWithSetting called testEvents. So now, m_eventsWithSetting=testEventsWithSetting.
            m_listEventsWithSetting.Add(testEventsWithSetting);//Add the list testEventsWithSetting which is a copy of m_eventsWithSetting and which contains the last record. We do a copy because otherwise, it is added by reference.

            #endregion

            #region Add the last record to the list of events serializable
            //Add the last record to the m_listeventsSerializable which contains the all lists of events.
            List<MacroEventSerializable> testEventsSerializable = new List<MacroEventSerializable>(m_eventsSerializable);//Make a copy of list m_events called testEventsSerializable. So now, m_events=testEventsSerializable.
            m_listeventsSerializable.Add(testEventsSerializable);//Add the list testEventsSerializable which is a copy of m_events and which contains the last record. We do a copy because otherwise, it is added by reference.
            m_eventsSerializable.Clear();
            #endregion
        }


        //Event when the user click on the playBack button
        public void playBackMacroButton_Click(object sender, EventArgs e)
        {
            #region Disable unused button when we Playback a Record
            disableUnusedButtonWhenPlaybackRecord();//Disable unused button when we Playback Record
            #endregion

            #region Parse the radio buttons to know which one is checked and therefore which event playback
            int numberIDSelected = 0;
            //For each component in ScrollPanel of FormModePersonalize Form
            foreach (object o in ScrollPanel.Controls)
            {
                //If the component is a RadioButton (i.e: If the type of component is a RadioButton).
                if (o is System.Windows.Forms.RadioButton)
                {
                    System.Windows.Forms.RadioButton currentRadioButton = (System.Windows.Forms.RadioButton)o;//Stock the component (which is a RadioButton) on a variable called currentRadioButton.
                    //If the current RadioButton is checked
                    if (currentRadioButton.Checked == true)
                    {
                        break;
                    }
                    else
                    {
                        numberIDSelected++;
                    }
                }
            }
            #endregion

            #region Playback the event that the user has chosen
            //Playback the event that the user has chosen
            foreach (MacroEvent macroEvent in m_listEventsWithSetting[numberIDSelected])
            {
                Thread.Sleep(macroEvent.TimeSinceLastEvent);//Stop the timer until that the macroEvent stop

                //Switch the type of MacroEvent read in the m_listEventsWithSetting[numberIDSelected]
                switch (macroEvent.MacroEventType)
                {
                    //If the MacroEvent read in the m_listEventsWithSetting[numberIDSelected] is of type MouseMove, then simulate the same movement of mouse.
                    case MacroEventType.MouseMove:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;
                            MouseSimulator.X = mouseArgs.X;
                            MouseSimulator.Y = mouseArgs.Y;
                        }
                        break;

                    //If the MacroEvent read in the m_listEventsWithSetting[numberIDSelected] is of type MouseDown, then simulate the same clic of mouse.
                    case MacroEventType.MouseDown:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;
                            MouseSimulator.MouseDown(mouseArgs.Button);
                        }
                        break;

                    //If the MacroEvent read in the m_listEventsWithSetting[numberIDSelected] is of type MouseUp, then simulate the same action of mouse.
                    case MacroEventType.MouseUp:
                        {
                            MouseEventArgs mouseArgs = (MouseEventArgs)macroEvent.EventArgs;
                            MouseSimulator.MouseUp(mouseArgs.Button);
                        }
                        break;

                    //If the MacroEvent read in the m_listEventsWithSetting[numberIDSelected] is of type KeyDown, then simulate the same action of keyboard.
                    case MacroEventType.KeyDown:
                        {
                            KeyEventArgs keyArgs = (KeyEventArgs)macroEvent.EventArgs;
                            KeyboardSimulator.KeyDown(keyArgs.KeyCode);
                        }
                        break;

                    //If the MacroEvent read in the m_listEventsWithSetting[numberIDSelected] is of type KeyUp, then simulate the same action of keyboad.
                    case MacroEventType.KeyUp:
                        {
                            KeyEventArgs keyArgs = (KeyEventArgs)macroEvent.EventArgs;
                            KeyboardSimulator.KeyUp(keyArgs.KeyCode);
                        }
                        break;
                    
                    //Otherwise, break.
                    default:
                        break;
                }
            }
            #endregion

            #region Enable used button when we Playback Record Stop
            enableUsedButtonWhenPlaybackRecordStop();//Enable used button when we Playback Record Stop
            #endregion
        }

        //Event when the user click on the recognition button
        public void speechButton_Click(object sender, EventArgs e)
        {
            #region if the speech recognition is not activate, try to activate the speech recognition in continious mode
            //if the speech recognition is not activate
            if (!m_speechRecognitionActivate)
            {
                //try to activate the speech recognition in continious mode
                try
                {
                    m_ASR.getm_ASR().RecognizeAsync(RecognizeMode.Multiple);//Activate the speech recognition in continious mode
                    m_speechRecognitionActivate = true;//Update the attribute which store if the speech recognition is activate or not
                    speechButton.Image = Properties.Resources.FreeSpeechRecognitionIcon;//Update button image to show that the speech recognition is activate
                }
                catch { }
            }
            #endregion

            #region if the speech recognition is activate, try to desactivate the speech recognition
            //if the speech recognition is not activate
            else if (m_speechRecognitionActivate)
            {
                //try to activate the speech recognition in continious mode
                try
                {
                    m_ASR.getm_ASR().RecognizeAsyncStop();//Desactivate the speech recognition in continious mode
                    m_speechRecognitionActivate = false;//Update the attribute which store if the speech recognition is activate or not
                    speechButton.Image = Properties.Resources.FreeNoSpeechRecognitionIcon;//Update button image to show that the speech recognition is not activate
                }
                catch { }
            }
            #endregion
        }

        #endregion

        #region Methods to Import/Export Several Events

        #region Methods to Import Several Events
        public void ImportSeveralEventsButton_Click(object sender, EventArgs e)
        {
            Stream streamToDeserializeEvents;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((streamToDeserializeEvents = openFileDialog1.OpenFile()) != null)
                {
                    // Code to write the stream goes here.
                    XmlSerializer eventSerialisation = new XmlSerializer(typeof(List<List<MacroEventSerializable>>));
                    List<List<MacroEventSerializable>> listEventsImported = (List<List<MacroEventSerializable>>) eventSerialisation.Deserialize(streamToDeserializeEvents);

                    for(int i=0; i<listEventsImported.Count; i++)
                    {
                        List<MacroEvent> listMacroEventRecreated = new List<MacroEvent>();
                        for (int j = 0; j < listEventsImported[i].Count; j++)
                        {
                            EventArgs currentMacroEvent;
                            //if the Event is a Mouse action
                            if(listEventsImported[i][j].MacroEventType != MacroEventType.KeyDown && listEventsImported[i][j].MacroEventType != MacroEventType.KeyUp)
                            {
                                currentMacroEvent = new MouseEventArgs(listEventsImported[i][j].m_button, listEventsImported[i][j].m_clicks, listEventsImported[i][j].m_x, listEventsImported[i][j].m_y, listEventsImported[i][j].m_delta);
                            }
                            //if the Event is a Keyboard action
                            else
                            {
                                currentMacroEvent = new KeyEventArgs(listEventsImported[i][j].m_KeyData);
                            }
                            MacroEvent currentEvent = new MacroEvent(listEventsImported[i][j].MacroEventType, currentMacroEvent, listEventsImported[i][j].TimeSinceLastEvent);
                            listMacroEventRecreated.Add(currentEvent);
                        }
                        m_listevents.Add(listMacroEventRecreated);
                        m_listEventsWithSetting.Add(listMacroEventRecreated);
                    }
                    m_listeventsSerializable.AddRange(listEventsImported);

                    #region For each Event imported, create a new RadioButton dynamically
                    if (m_numberID == 0)
                    {
                        #region Enable playBackMacroButton Button
                        playBackMacroButton.Enabled = true;//Enable playBackMacroButton Button
                        #endregion
                    }
                    //For each Event imported, create a new RadioButton dynamically
                    for (int i = 0; i<listEventsImported.Count; i++)
                    {
                        #region Create new RadioButton dynamically for the event that we import

                        #region Create new RadioButton variable, set it is name dynamically and update variable
                        //Create new RadioButton dynamically for the event that we import
                        m_numberID++;//Increment the number of event
                        System.Windows.Forms.RadioButton radb = new System.Windows.Forms.RadioButton();//Create a RadioButton
                        radb.Text = "Event" + m_numberID;//Set the text of RadioButton with the name Event following by the number of events
                        Point lastLocationRadioButton = new System.Drawing.Point(0, 0);//Create a variable of type Point
                        #endregion

                        #region Uncheck all Radio button and check the last Radio button that we want to add dynamically
                        //For each component in ScrollPanel of FormModePersonalize Form, we uncheck all Radio button
                        foreach (object o in ScrollPanel.Controls)
                        {
                            //If the component is a RadioButton (i.e: If the type of component is a RadioButton).
                            if (o is System.Windows.Forms.RadioButton)
                            {
                                System.Windows.Forms.RadioButton currentRadioButton = (System.Windows.Forms.RadioButton)o;//Stock the component (which is a RadioButton) on a variable called currentRadioButton.
                                lastLocationRadioButton = currentRadioButton.Location;//Set the variable lastLocationRadioButton with the location of the current RadioButton.
                                                                                      //If the current RadioButton is checked
                                if (currentRadioButton.Checked == true)
                                {
                                    currentRadioButton.Checked = false;//Uncheck all Radio button exept the last one.
                                }
                            }
                        }
                        radb.Checked = true;//Check the last Radio button
                        #endregion

                        #region Set the position of the new Radio button
                        //If it is the first record
                        if (m_numberID == 1)
                        {
                            radb.Location = new System.Drawing.Point(10, 0);//Set the position of RadioButton in the FormModePersonalize Form
                        }
                        //If it is not the first record
                        else
                        {
                            radb.Location = new System.Drawing.Point(10, lastLocationRadioButton.Y + 30);//Set the position of RadioButton in the FormModePersonalize Form
                        }
                        #endregion

                        #region Add the new Radio button to the ScrollPanel
                        ScrollPanel.Controls.Add(radb);//Add RadioButton in panel called ScrollPanel in FormModePersonalize Form
                        #endregion

                        #region Add the new Radio button to the list m_listOfRadioButton
                        m_listOfRadioButton.Add(radb);//Add the new Radio button to the list m_listOfRadioButton
                        #endregion

                        #endregion
                    }
                    #endregion

                    #region Enable ExportSeveralEventsButton Button
                    ExportSeveralEventsButton.Enabled = true;//Enable the button to export records
                    #endregion

                    #region Enable EventSettingButton Button
                    EventSettingButton.Enabled = true;//Enable the button to setting speed record
                    #endregion

                    streamToDeserializeEvents.Close();
                }
            }
        }
        #endregion

        #region Methods to Export Several Events
        public void ExportSeveralEventsButton_Click(object sender, EventArgs e)
        {
            //if there are no events that have been created yet, we cannot export events. So, we create a error message box.
            if (m_listevents.Count() == 0)
            {
                #region If there are no events that have been created yet, we cannot export events. So, we create a error message box.
                // Configure the message box to be displayed
                string messageBoxText = "Cannot export events because there are no event that have been created yet. Create at least one event to be able to export events!";
                string caption = "Error";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);//Show message box error to inform user that the metod playBackMacroButton_Click fail
                #endregion
            }
            else
            {
                #region Create a chooseEventsToExport Form to choose which events the user want export
                //Create a chooseEventsToExport Form to choose which events the user want import.
                ChooseEventsToExport chooseEventsToExport = new ChooseEventsToExport(this);//Create a chooseEventsToExport Form to choose which events the user want import.
                chooseEventsToExport.ShowDialog();//Show in modal mode the chooseEventsToExport Form.
                #endregion

                #region Export the chosen events.
                //If m_exportEventsConfiration attribute is equal to true, import events.
                if (m_exportEventsConfiration)
                {
                    List<List<MacroEventSerializable>> listeventsSerializableChosenToExport = new List<List<MacroEventSerializable>>();
                    for (int i = 0; i < m_listOfEventsChosenToExport.Count; i++)
                    {
                        listeventsSerializableChosenToExport.Add(m_listeventsSerializable[m_listOfEventsChosenToExport[i]]);
                    }

                    Stream streamToSerializeEvents;
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
                    saveFileDialog1.FilterIndex = 1;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        if ((streamToSerializeEvents = saveFileDialog1.OpenFile()) != null)
                        {
                            Console.WriteLine(m_listeventsSerializable);
                            // Code to write the stream goes here.
                            XmlSerializer eventSerialisation = new XmlSerializer(typeof(List<List<MacroEventSerializable>>));
                            eventSerialisation.Serialize(streamToSerializeEvents, listeventsSerializableChosenToExport);

                            streamToSerializeEvents.Close();
                        }
                    }

                    m_listOfEventsChosenToExport.Clear();//Clear the list m_listOfEventsChosenToExport
                    m_exportEventsConfiration = false;//Set the m_exportEventsConfiration attribute to false
                }
                #endregion
            }
        }
        #endregion

        #endregion

        #region Methods to configure the setting of events
        public void EventSettingButton_Click(object sender, EventArgs e)
        {
            //If there are no events that have been created yet, we cannot configure the setting of events. So, we create a error message box.
            if (m_listevents.Count() == 0)
            {
                #region If there are no events that have been created yet, we cannot configure the setting of events. So, we create a error message box.
                // Configure the message box to be displayed
                string messageBoxText = "Cannot configure the setting of events because there are no event that have been created yet. Create at least one event to be able to configure the settings of events!";
                string caption = "Error";
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);//Show message box error to inform user that the metod EventSettingButton_Click fail
                #endregion
            }
            //If there are at least one event created
            else
            {
                #region Create a ChooseEventsToSetting Form to configure the setting of events chosen by user
                //Create a ChooseEventsToSetting Form to configure the setting of events chosen by user.
                ChooseEventsToSetting chooseEventSettingForm = new ChooseEventsToSetting(this);//Create a ChooseEventsToSetting Form to configure the setting of events chosen by user.
                chooseEventSettingForm.ShowDialog();//Show in modal mode the ChooseEventsToSetting Form.
                #endregion

                #region Open the setting Form to setting the chosen events.
                //If m_settingEventsConfiration attribute is equal to true, open the setting Form to setting the chosen events.
                if (m_settingEventsConfiration)
                {
                    #region Create a ChooseEventsToSetting Form to configure the setting of events chosen by user
                    //Create a ChooseEventsToSetting Form to configure the setting of events chosen by user.
                    EventSettingForm eventSettingForm = new EventSettingForm(this);//Create a ChooseEventsToSetting Form to configure the setting of events chosen by user.
                    eventSettingForm.ShowDialog();//Show in modal mode the ChooseEventsToSetting Form.
                    #endregion

                    m_listOfEventsChosenToSetting.Clear();//Clear the list m_listOfEventsChosenToSetting
                    m_settingEventsConfiration = false;//Set the m_settingEventsConfiration attribute to false
                }
                #endregion
            }
        }
        #endregion

        #region Methods to check/uncheck a RadioButton by its event id

        #region Methods to check a RadioButton by its event id
        //Check a RadioButton by its event id
        public void checkRadioButtonByItsEventId(int idOfEvent)
        {
            #region Parse the radio buttons to know which one is checked and therefore which event playback
            int currentIdRadioButton = 1;
            //For each component in ScrollPanel of FormModePersonalize Form
            foreach (object o in ScrollPanel.Controls)
            {
                //If the component is a RadioButton (i.e: If the type of component is a RadioButton).
                if (o is System.Windows.Forms.RadioButton)
                {
                    System.Windows.Forms.RadioButton currentRadioButton = (System.Windows.Forms.RadioButton)o;//Stock the component (which is a RadioButton) on a variable called currentRadioButton.

                    if (currentIdRadioButton != idOfEvent)
                    {
                        //If the current RadioButton is checked
                        if (currentRadioButton.Checked == true)
                        {
                            currentRadioButton.Checked = false;
                            currentIdRadioButton++;
                        }
                        else
                        {
                            currentIdRadioButton++;
                        }
                    }
                    else
                    {
                        //If the current RadioButton is checked
                        if (currentRadioButton.Checked == false)
                        {
                            currentRadioButton.Checked = true;
                            currentIdRadioButton++;
                        }
                        else
                        {
                            currentIdRadioButton++;
                        }
                    }
                }
            }
            #endregion
        }
        #endregion

        #region Methods to uncheck a RadioButton by its event id
        //Uncheck a RadioButton by its event id
        public void UncheckRadioButtonByItsEventId(int idOfEvent)
        {
            getRadioButtonByEventId(idOfEvent).Checked = false;
        }
        #endregion

        #endregion

        #endregion
    }
}
