/*
* BinaryDiff class by SamsamTS
* Copyright (c) 2024 SamsamTS
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
* of the Software, and to permit persons to whom the Software is furnished to do
* so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
using System;
using System.Collections.Generic;

public class BinaryDiff
{
    /// <summary>
    /// Represents a section
    /// </summary>
    public class Section
    {
        /// <summary>
        /// The altered data
        /// </summary>
        public Byte[] data;
        /// <summary>
        /// The starting point in the original file
        /// </summary>
        public Int32 start;
        /// <summary>
        /// The end point in the original file
        /// </summary>
        public Int32 end;
        /// <summary>
        /// The number of bytes added or removed
        /// </summary>
        public Int32 diff;
        /// <summary>
        /// Copies the section
        /// </summary>
        /// <returns>A copy of the section</returns>
        public Section Copy()
        {
            Section copy = new Section()
            {
                data = new byte[data.Length],
                start = start,
                end = end,
                diff = diff
            };
            Array.Copy(data, copy.data, data.Length);
            return copy;
        }
        /// <summary>
        /// Checks is the other section is identical
        /// </summary>
        /// <param name="other">The other section</param>
        /// <returns>true if the sections are identical</returns>
        public Boolean Equals(Section other)
        {
            if (other.start != start || other.end != end || other.diff != diff || other.data.Length != data.Length)
                return false;
            /*for (Int32 i = 0; i < data.Length; i++)
            {
                if (other.data[i] != data[i])
                    return false;
            }*/
            return true;
        }
    }

    /// <summary>
    /// The list sections containing the differences with the original file
    /// </summary>
    public List<Section> Sections { get; private set; }
    /// <summary>
    /// The number of sections
    /// </summary>
    public Int32 Count { get => Sections.Count; }
    /// <summary>
    /// The total bytes added or removed from the original file
    /// </summary>
    public Int32 TotalDiff { get; private set; }
    /// <summary>
    /// Define the minimum space in bytes between sections.
    /// For exemple with a padding of 4, if two changes are within 4 bytes of each other, only one section wil be generated.
    /// A typical value used in binary files are 32bits interger (4 bytes), a padding of 4 is recommended
    /// A greater padding can reduces the number of sections at the cost of granularity.
    /// </summary>
    public Int32 Padding { get; private set; }

    /// <summary>
    /// Creates an empty Diff
    /// </summary>
    public BinaryDiff(Int32 padding = 4)
    {
        Padding = Math.Max(1, padding);
        TotalDiff = 0;
        Sections = new List<Section>();
    }

    /// <summary>
    /// Creates a Diff that stores the difference necessary to go from fileA to fileB
    /// </summary>
    /// <param name="fileA">The original file</param>
    /// <param name="fileB">The modified file</param>
    /// <param name="fileB">Minimum padding between sections</param>
    public unsafe BinaryDiff(Byte[] fileA, Byte[] fileB, Int32 padding = 4)
    {
        Padding = Math.Max(1, padding);
        TotalDiff = 0;
        Sections = new List<Section>();
        Int32 delta = 0;

        fixed (Byte* a = fileA, b = fileB)
        {
            for (Int32 i = 0; i < fileA.Length; i++)
            {
                if (i + delta >= fileB.Length)
                {
                    // Reached the end of fileB prematurally
                    Section section = new Section
                    {
                        start = i,
                        end = fileA.Length,
                        diff = i - fileA.Length,
                        data = new Byte[0]
                    };
                    Sections.Add(section);
                    TotalDiff += section.diff;
                    return;
                }
                if (fileA[i] != fileB[i + delta])
                {
                    // Diff found
                    Section section = new Section();
                    FindSectionEnd(a, i, fileA.Length, out Int32 endA, b, i + delta, fileB.Length, out Int32 endB);
                    section.start = i;
                    section.end = endA;
                    Int32 l = endB - (i + delta);
                    if (l > 0)
                    {
                        // Data added or replaced
                        section.data = new Byte[l];
                        Array.Copy(fileB, i + delta, section.data, 0, l);
                    }
                    else
                    {
                        // Data removed
                        section.data = new Byte[0];
                    }
                    section.diff = section.data.Length - (section.end - section.start);
                    TotalDiff += section.diff;
                    Sections.Add(section);
                    i = endA - 1;
                    delta = endB - i - 1;
                }
            }
            Int32 len = fileB.Length - (fileA.Length + delta);
            if (len > 0)
            {
                // Data added at the end
                Section section = new Section
                {
                    start = fileA.Length,
                    end = fileA.Length + len,
                    diff = len,
                    data = new Byte[len]
                };
                Array.Copy(fileB, fileA.Length + delta, section.data, 0, len);
                Sections.Add(section);
                TotalDiff += section.diff;
            }
        }
    }

    /// <summary>
    /// Attempts to merge two Diff, ideally created from the same origin file
    /// If conflicts are detected the merge is cancelled
    /// </summary>
    /// <param name="diff">The Diff to merge</param>
    /// <returns>true if the merge succeeded</returns>
    public Boolean TryMerge(BinaryDiff diff)
    {
        if (diff.Count == 0)
            return true;
        if (Count == 0)
        {
            Sections = new List<Section>(diff.Sections);
            TotalDiff = diff.TotalDiff;
            return true;
        }
        List<Section> newSections = new List<Section>();
        foreach (Section section in Sections)
        {
            newSections.Add(section.Copy());
        }
        Int32 newTotal = TotalDiff;
        foreach (Section sectionA in diff.Sections)
        {
            Boolean canMerge = true;
            Int32 at = 0;
            foreach (Section sectionB in newSections)
            {
                if (sectionA.Equals(sectionB))
                {
                    // Identical section
                    canMerge = false;
                    break;
                }
                if (sectionA.end == sectionB.start && sectionA.start < sectionB.end)
                {
                    // Append at start
                    Byte[] bytes = new Byte[sectionA.data.Length + sectionB.data.Length];
                    Array.Copy(sectionA.data, 0, bytes, 0, sectionA.data.Length);
                    Array.Copy(sectionB.data, 0, bytes, sectionA.data.Length, sectionB.data.Length);
                    sectionB.data = bytes;
                    sectionB.diff += sectionA.diff;
                    newTotal += sectionA.diff;
                    canMerge = false;
                    break;
                }
                if (sectionA.start == sectionB.end && sectionA.end > sectionB.start)
                {
                    // Append at end
                    Byte[] bytes = new Byte[sectionA.data.Length + sectionB.data.Length];
                    Array.Copy(sectionB.data, 0, bytes, 0, sectionB.data.Length);
                    Array.Copy(sectionA.data, 0, bytes, sectionB.data.Length, sectionA.data.Length);
                    sectionB.data = bytes;
                    sectionB.diff += sectionA.diff;
                    newTotal += sectionA.diff;
                    canMerge = false;
                    break;
                }
                if (sectionA.start > sectionB.start) at++;
                if ((sectionA.start >= sectionB.start && sectionA.start < sectionB.end) ||
                    (sectionA.end > sectionB.start && sectionA.end <= sectionB.end))
                {
                    // Sections overlap, cancel the merge
                    return false;
                }
            }
            if (canMerge)
            {
                // Insert
                newSections.Insert(at, sectionA);
                newTotal += sectionA.diff;
            }
        }
        Sections = newSections;
        TotalDiff = newTotal;
        return true;
    }

    /// <summary>
    /// Applies the difference to a copy of the file. 
    /// </summary>
    /// <param name="file">The file onto which the difference shall be applied</param>
    /// <returns>A copy of the file with the difference applied</returns>
    public Byte[] Apply(Byte[] file)
    {
        Byte[] bytes = new Byte[file.Length + TotalDiff];
        Int32 i = 0;
        Int32 delta = 0;
        foreach (Section section in Sections)
        {
            Array.Copy(file, i, bytes, i + delta, section.start - i);
            i = section.start;
            if (section.data.Length > 0)
                Array.Copy(section.data, 0, bytes, i + delta, section.data.Length);
            i = section.end;
            delta += section.diff;
        }
        if (bytes.Length - (i + delta) > 0)
            Array.Copy(file, i, bytes, i + delta, bytes.Length - (i + delta));
        return bytes;
    }

    #region private
    private unsafe void FindSectionEnd(Byte* a, Int32 startA, Int32 lenA, out Int32 endA, Byte* b, Int32 startB, Int32 lenB, out Int32 endB)
    {
        Int32 maxA = lenA - startA - Padding;
        Int32 maxB = lenB - startB - Padding;
        endA = lenA;
        endB = lenB;
        Int32 maxPos = (lenA - startA) + (lenB - startB);
        a += startA;
        b += startB;
        for (Int32 i = 0; i < maxA; i++)
        {
            if (i >= maxPos) break;
            for (Int32 j = 0; j < maxB; j++)
            {
                if (i + j >= maxPos) break;
                if (Equals(a + i, b + j, Padding))
                {
                    endA = startA + i;
                    endB = startB + j;
                    maxPos = i + j;
                }
            }
        }
    }
    private unsafe bool Equals(Byte* a, Byte* b, Int32 len)
    {
        for (Int32 i = 0; i < len; i++)
            if (a[i] != b[i]) return false;

        return true;
    }
    #endregion
}
