﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLibrary
{

    public interface IPresenterCommon
    {
        void AddObject();
        void EditObject();
        void DeleteObject();
        void Search(string searchTerm);
    }
}
