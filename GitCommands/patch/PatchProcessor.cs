            string input;
            while ((input = textReader.ReadLine()) != null)
                if (IsStartOfANewPatch(input))
                    ExtractPatchFilenames(input, patch);
                        if (IsStartOfANewPatch(input))
                        SetPatchType(input, patch);

                        if (!IsIndexLine(input))
                        {
                            if (textReader.ReadLine() == null)
                            {
                            }
                        }
                       if (IsUnlistedBinaryFileDelete(input))
                                throw new FormatException("Change not parsed correct: " + input);
                        if (IsUnlistedBinaryNewFile(input))
                                throw new FormatException("Change not parsed correct: " + input);
                        if (IsBinaryPatch(input))
                    ValidateInput(input, patch, gitPatch);
                }
                if (patch != null)
                    patch.AppendTextLine(input);
            }
            return patches;
        }
        private static bool IsIndexLine(string input)
        {
            return input.StartsWith("index ");
        }
        private static void ValidateInput(string input, Patch patch, bool gitPatch)
        {
            //The previous check checked only if the file was binary
            //--- /dev/null
            //means there is no old file, so this should be a new file
            if (IsOldFileMissing(input))
            {
                if (gitPatch && patch.Type != Patch.PatchType.NewFile)
                    throw new FormatException("Change not parsed correct: " + input);
            }

            //line starts with --- means, old file name
            if (HasOldFileName(input))
            {
                if (gitPatch && patch.FileNameA != (input.Substring(6).Trim()))
                    throw new FormatException("Old filename not parsed correct: " + input);
            }
            if (IsNewFileMissing(input))
            {
                if (gitPatch && patch.Type != Patch.PatchType.DeleteFile)
                    throw new FormatException("Change not parsed correct: " + input);
            }
            //line starts with +++ means, new file name
            //we expect a new file now!
            if (input.StartsWith("+++ ") && !IsNewFileMissing(input))
            {
                Match regexMatch = Regex.Match(input, "[+]{3}[ ][\\\"]{0,1}[b]/(.*)[\\\"]{0,1}");
                if (gitPatch && patch.FileNameB != (regexMatch.Groups[1].Value.Trim()))
                    throw new FormatException("New filename not parsed correct: " + input);
            }
        }
        private static bool IsNewFileMissing(string input)
        {
            return input.StartsWith("+++ /dev/null");
        }
        private static bool HasOldFileName(string input)
        {
            return input.StartsWith("--- a/");
        }
        private static bool IsOldFileMissing(string input)
        {
            return input.StartsWith("--- /dev/null");
        }
        private static bool IsBinaryPatch(string input)
        {
            return input.StartsWith("GIT binary patch");
        }
        private static bool IsUnlistedBinaryFileDelete(string input)
        {
            return input.StartsWith("Binary files a/") && input.EndsWith(" and /dev/null differ");
        }
        private static bool IsUnlistedBinaryNewFile(string input)
        {
            return input.StartsWith("Binary files /dev/null and b/") && input.EndsWith(" differ");
        }
        private static void ExtractPatchFilenames(string input, Patch patch)
        {
            Match match = Regex.Match(input,
                                      "[ ][\\\"]{0,1}[a]/(.*)[\\\"]{0,1}[ ][\\\"]{0,1}[b]/(.*)[\\\"]{0,1}");
            patch.FileNameA = match.Groups[1].Value;
            patch.FileNameB = match.Groups[2].Value;
        }
        private static bool IsStartOfANewPatch(string input)
        {
            return input.StartsWith("diff --git ");
        }
        private static void SetPatchType(string input, Patch patch)
        {
            if (input.StartsWith("new file mode "))
                patch.Type = Patch.PatchType.NewFile;
            else if (input.StartsWith("deleted file mode "))
                patch.Type = Patch.PatchType.DeleteFile;
            else
                patch.Type = Patch.PatchType.ChangeFile;