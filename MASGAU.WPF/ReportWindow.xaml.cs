﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using MASGAU.Analyzer;
using MVC.Translator;
using MVC.WPF;
using Translator;
using Translator.WPF;
namespace MASGAU {
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : NewWindow {
        AAnalyzer analyzer;
        public bool SentOrSaved {
            get {
                return uploadBtn.Sent || saved;
            }
        }
		public ReportWindow(AAnalyzer analyzer, AViewWindow owner)
            : base(owner) {
            InitializeComponent();
            TranslationHelpers.translateWindow(this);
            this.Icon = owner.Icon;
            this.analyzer = analyzer;
            reportTxt.Text = analyzer.report;

            uploadBtn.To = Core.submission_email;
            uploadBtn.Subject = "Game Data - " + analyzer.game.Title;
            uploadBtn.Message = analyzer.report;
            uploadBtn.Source = Core.settings;
        }


        private bool saved = false;
        private void saveBtn_Click(object sender, RoutedEventArgs e) {
            System.Windows.Forms.SaveFileDialog save = new System.Windows.Forms.SaveFileDialog();
            save.DefaultExt = "txt";
            save.Filter = Strings.GetLabelString("TxtFileDescriptionPlural") + "|*.txt|" + Strings.GetLabelString("AllFileDescriptionPlural") + "|*";
            save.Title = Strings.GetLabelString("SaveReportQuestion");

            if (String.IsNullOrEmpty(AAnalyzer.LastSavePath))
                save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            else
                save.InitialDirectory = AAnalyzer.LastSavePath;

            save.FileName = analyzer.game.Name;
			if (!String.IsNullOrEmpty(analyzer.game.id.game.OS)) {
				save.FileName += "." + analyzer.game.id.game.OS;
			}
			if (!String.IsNullOrEmpty(analyzer.game.id.game.Platform)) {
				save.FileName += "." + analyzer.game.id.game.Platform;
			}
			if (!String.IsNullOrEmpty(analyzer.game.id.game.Media)) {
				save.FileName += "." + analyzer.game.id.game.Media;
			}
			if (!String.IsNullOrEmpty(analyzer.game.id.game.Release)&&analyzer.game.id.game.Release!="Custom") {
				save.FileName += "." + analyzer.game.id.game.Release;
			}
			if (!String.IsNullOrEmpty(analyzer.game.id.game.Region)) {
				save.FileName += "." + analyzer.game.id.game.Region;
			}
			save.FileName += ".txt";


            if (save.ShowDialog(this.GetIWin32Window()) != System.Windows.Forms.DialogResult.Cancel) {
                AAnalyzer.LastSavePath = Path.GetDirectoryName(save.FileName);
                try {
                    StreamWriter writer = File.CreateText(save.FileName);
                    writer.Write(reportTxt.Text);
                    writer.Close();
                    saved = true;
                } catch (Exception ex) {
                    TranslatingMessageHandler.SendError("WriteError", ex, save.FileName);
                }
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        private void reportTxt_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            uploadBtn.Message = reportTxt.Text;
        }


    }
}