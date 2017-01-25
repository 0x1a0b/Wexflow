﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wexflow.Core;
using System.Xml.Linq;
using System.IO;
using System.Threading;

namespace Wexflow.Tasks.FilesRemover
{
    public class FilesRemover:Task
    {
        public FilesRemover(XElement xe, Workflow wf)
            : base(xe, wf)
        {
        }

        public override TaskStatus Run()
        {
            this.Info("Removing files...");
            
            bool success = true;
            bool atLeastOneSucceed = false;

            FileInf[] files = this.SelectFiles();
            for (int i = files.Length - 1; i > -1; i--)
            {
                FileInf file = files[i];

                try
                {
                    File.Delete(file.Path);
                    this.Workflow.FilesPerTask[file.TaskId].Remove(file);
                    this.InfoFormat("File removed: {0}", file.Path);
                    success &= true;
                    if (!atLeastOneSucceed) atLeastOneSucceed = true;
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    this.ErrorFormat("An error occured while deleting the file {0}", e, file.Path);
                    success &= false;
                }
            }

            Status status = Status.Success;

            if (!success && atLeastOneSucceed)
            {
                status = Status.Warning;
            }
            else if (!success)
            {
                status = Status.Error;
            }

            this.Info("Task finished.");
            return new TaskStatus(status, false);
        }
    }
}
