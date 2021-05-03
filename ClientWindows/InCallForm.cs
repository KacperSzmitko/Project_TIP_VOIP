﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;

namespace ClientWindows
{
    public delegate void ByteCallback(byte[] b);
    public partial class InCallForm : Form
    {

        private Id callId = null;
        private Boolean callStopped = false;
        private Call call = null;

        private int i = 0;


        public InCallForm()
        {
            InitializeComponent();
        }

        public InCallForm(Call c)
        {
            
            this.call = c;
            this.callId = new Id(c.callId);
            InitializeComponent();
            String usersListStr = "";
            foreach (String u in call.usernames)
            {
                if (!usersListStr.Equals(""))
                {
                    usersListStr += ", ";
                }
                usersListStr += u;
            }
            this.callUsersList_label.Text = usersListStr;
            //Task.Run(progBarIncrease);

            ByteCallback callback = incomingTraffic;
            CallProcessing.ReceiveMsgCallback = callback;
            CallProcessing.Start();
            Task.Run(sentBytes);
            Program.isInCall = true;
        }

        public InCallForm(Id id, Username user)
        {
            this.callId = id;
            InitializeComponent();
            this.callUsersList_label.Text = user.username;
            //Task.Run(progBarIncrease);

            ByteCallback callback = incomingTraffic;
            CallProcessing.ReceiveMsgCallback = callback;
            CallProcessing.Start();
            Task.Run(sentBytes);
            Program.isInCall = true;
        }

        private void InCallForm_Load(object sender, EventArgs e)
        {

        }

        private void leaveCall_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InCallForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.callId == null) return;
            callStopped = true;
            CallProcessing.SendMessages(null);
            CallProcessing.SendMessages(null);
            CallProcessing.SendMessages(null);
            CallProcessing.Stop();
            LoggedInService.declineCall(this.callId);
            Program.isInCall = false;
        }

        private void incomingTraffic_label_Click(object sender, EventArgs e)
        {
            
        }

        /*private void progBarIncrease()
        {
            if(incomingTraffic_bar.InvokeRequired)
            {
                incomingTraffic_bar.Invoke(new MethodInvoker(() => { progBarIncrease(); }));
                return;
            }
            incomingTraffic_bar.Visible = true;
            incomingTraffic_bar.Minimum = 0;
            incomingTraffic_bar.Maximum = 1;
            incomingTraffic_bar.Value = 1;
            incomingTraffic_bar.Step = 1;
            while(true)
            {
                if(incomingTraffic_bar.Value==1)
                    incomingTraffic_bar.Value = 0;
                else
                    incomingTraffic_bar.PerformStep();
                System.Threading.Thread.Sleep(500);
            }
            
        }*/

        public void incomingTraffic(byte[] b)
        {
            if (incomingMsg_label.InvokeRequired)
            {
                incomingMsg_label.Invoke(new MethodInvoker(() => { incomingTraffic(b); }));
            }
            else
            {
                incomingMsg_label.Text = b[0].ToString();
            }
        }

        public void sentBytes()
        {
            do
            {
                byte b = (byte)'Z';
                switch (i)
                {
                    case 0:
                        b = (byte)'A';
                        break;
                    case 1:
                        b = (byte)'B';
                        break;
                    case 2:
                        b = (byte)'C';
                        break;
                    case 3:
                        b = (byte)'D';
                        break;
                }

                CallProcessing.SendMessages(new byte[] { b });
                i++;
                if (i == 4)
                {
                    i = 0;
                }
                System.Threading.Thread.Sleep(100);
            } while (callStopped==false);
        }
    }
}
