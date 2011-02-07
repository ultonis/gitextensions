using System;
using System.Collections.Generic;
using System.IO;
using GitCommands;

namespace PatchApply
{
    public class PatchManager
    {
        private List<Patch> _patches = new List<Patch>();
        private readonly PatchProcessor _patchProcessor;

        public PatchManager()
            : this(new PatchProcessor())
        {
        }

        public PatchManager(PatchProcessor patchProcessor)
        {
            _patchProcessor = patchProcessor;
        }

        public string PatchFileName { get; set; }

        public string DirToPatch { get; set; }

        public List<Patch> Patches
        {
            get { return _patches; }
            set { _patches = value; }
        }

        public void LoadPatch(string text, bool applyPatch)
        {
            using (var stream = new StringReader(text))
            {
                LoadPatchStream(stream, applyPatch);
            }
        }

        public void LoadPatchFile(bool applyPatch)
        {
            using (var re = new StreamReader(PatchFileName, Settings.Encoding))
            {
                LoadPatchStream(re, applyPatch);
            }
        }

        private void LoadPatchStream(TextReader reader, bool applyPatch)
        {
            _patches = _patchProcessor.CreatePatchesFromReader(reader);

            if (!applyPatch)
                return;

            foreach (Patch patchApply in _patches)
            {
                if (patchApply.Apply)
                    patchApply.ApplyPatch();
            }
        }
    }
}