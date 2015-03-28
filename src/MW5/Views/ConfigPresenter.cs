﻿using System;
using System.Diagnostics;
using MW5.Configuration;
using MW5.Plugins.Interfaces;
using MW5.Plugins.Mvp;
using MW5.Plugins.Services;
using MW5.Views.Abstract;

namespace MW5.Views
{
    internal class ConfigPresenter: BasePresenter<IConfigView>
    {
        private readonly IAppContext _context;
        private readonly IConfigView _view;
        private readonly IConfigService _configService;
        private readonly IPluginManager _manager;
        private readonly IMessageService _messageService;

        public ConfigPresenter(IAppContext context, IConfigView view, IConfigService configService, IPluginManager manager,
                               IMessageService messageService)
            : base(view)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (view == null) throw new ArgumentNullException("view");
            if (configService == null) throw new ArgumentNullException("configService");
            if (manager == null) throw new ArgumentNullException("manager");
            if (messageService == null) throw new ArgumentNullException("messageService");

            _context = context;
            _view = view;
            _configService = configService;
            _manager = manager;
            _messageService = messageService;

            InitPages();

            _view.Initialize();

            view.OpenFolderClicked += view_OpenFolderClicked;
            view.SaveClicked += view_SaveClicked;
        }

        private void view_SaveClicked()
        {
            ApplySettings();
            bool result = _configService.Save();
            if (result)
            {
                _messageService.Info("Configuration was saved successfully.");
            }
        }

        private void view_OpenFolderClicked()
        {
            string path = _configService.ConfigPath;
            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                _messageService.Warn("Failed to open folder: " + path + 
                Environment.NewLine + ex.Message);
            }
        }

        private void ApplySettings()
        {
            foreach (var page in _view.Pages)
            {
                page.Save();
            }
        }

        private void InitPages()
        {
            _view.Pages.Add(new GeneralConfigPage(_configService));
            _view.Pages.Add(new PluginsConfigPage(_configService, _manager, _context));
        }

        public override bool ViewOkClicked()
        {
            ApplySettings();
            _configService.Save();
            return true;
        }
    }
}
