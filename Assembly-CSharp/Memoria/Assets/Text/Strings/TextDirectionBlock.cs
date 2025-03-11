using System;
using System.Collections.Generic;
using UnityEngine;

namespace Memoria.Assets
{
    public class TextDirectionBlock
    {
        public Boolean ltr;
        public Int32 start; // Either start/end are different
        public Int32 end;
        public Boolean isTabBlock; // Tab blocks are isolated blocks containing only one [XTAB] tag
        public List<FFIXTextTag> tags = new List<FFIXTextTag>(); // or tags.Count > 0
        public LinkedListNode<TextDirectionBlock> nextByProgress = null;

        public Boolean registered = false;
        public Int32 vertexStart = 0;
        public Int32 vertexEnd = 0;
        public Single width = 0f;

        public List<DialogImage> images = new List<DialogImage>();

        private TextDirectionBlock(Boolean ltr, Int32 index, Boolean isTabBlock)
        {
            this.ltr = ltr;
            this.start = index;
            this.end = index;
            this.isTabBlock = isTabBlock;
        }

        public static void PrepareBlocks(UnicodeBIDI bidi, List<FFIXTextTag> tagList, out LinkedList<TextDirectionBlock> blockList, out LinkedListNode<TextDirectionBlock> firstBlockNode)
        {
            blockList = new LinkedList<TextDirectionBlock>();
            firstBlockNode = null;
            HashSet<FFIXTextTag> tagRegistered = new HashSet<FFIXTextTag>();
            for (Int32 i = 0; i < bidi.Directions.Count; i++)
            {
                Boolean blockLtr = bidi.Directions[i].ltr;
                Int32 blockRenderStart = blockLtr ? bidi.Directions[i].start : bidi.Directions[i].end;
                Int32 blockRenderEnd = blockLtr ? bidi.Directions[i].end : bidi.Directions[i].start;
                TextDirectionBlock block = new TextDirectionBlock(blockLtr, blockRenderStart, false);
                for (Int32 j = blockRenderStart; (blockLtr && j <= blockRenderEnd) || (!blockLtr && j >= blockRenderEnd); j += blockLtr ? 1 : -1)
                {
                    for (Int32 tagIndex = 0; tagIndex < tagList.Count; tagIndex++)
                    {
                        if (tagList[tagIndex].TextOffset != j || tagRegistered.Contains(tagList[tagIndex]))
                            continue;
                        if (j != block.start || block.isTabBlock || tagList[tagIndex].Code == FFIXTextTagCode.DialogX)
                        {
                            if (block.ltr)
                                block.end = j;
                            else
                                block.start = j;
                            if (block.start != block.end || block.tags.Count > 0)
                                blockList.AddLast(block);
                            block = new TextDirectionBlock(blockLtr, j, tagList[tagIndex].Code == FFIXTextTagCode.DialogX);
                        }
                        block.tags.Add(tagList[tagIndex]);
                        tagRegistered.Add(tagList[tagIndex]);
                    }
                    if (block.tags.Count > 0)
                    {
                        blockList.AddLast(block);
                        block = new TextDirectionBlock(blockLtr, j, false);
                    }
                }
                if (block.start != blockRenderEnd)
                {
                    if (block.ltr)
                        block.end = blockRenderEnd;
                    else
                        block.start = blockRenderEnd;
                    blockList.AddLast(block);
                }
            }
            for (LinkedListNode<TextDirectionBlock> blockNode = blockList.First; blockNode != null; blockNode = blockNode.Next)
            {
                TextDirectionBlock block = blockNode.Value;
                if (firstBlockNode == null || (block.start < firstBlockNode.Value.start && block.tags.Count == 0) || (block.start <= firstBlockNode.Value.start && block.tags.Count > 0))
                    firstBlockNode = blockNode;
                Int32 lastTagIndex = block.tags.Count > 0 ? tagList.IndexOf(block.tags[block.tags.Count - 1]) : -1;
                Boolean searchNextTag = block.tags.Count > 0 && lastTagIndex + 1 < tagList.Count && tagList[lastTagIndex].TextOffset == tagList[lastTagIndex + 1].TextOffset;
                Int32 closestDist = Int32.MaxValue;
                for (LinkedListNode<TextDirectionBlock> search = blockList.First; search != null; search = search.Next)
                {
                    if (search == blockNode)
                        continue;
                    if (searchNextTag)
                    {
                        if (search.Value.tags.Count > 0 && lastTagIndex + 1 == tagList.IndexOf(search.Value.tags[0]))
                        {
                            block.nextByProgress = search;
                            break;
                        }
                    }
                    else
                    {
                        Int32 dist = search.Value.start - block.end;
                        if (dist >= 0 && dist <= closestDist && (dist > 0 || block.tags.Count == 0 || search.Value.tags.Count == 0))
                        {
                            if (dist < closestDist)
                            {
                                block.nextByProgress = search;
                                closestDist = dist; // May be > 0 because there is a gap in case of new lines
                            }
                            else if (search.Value.tags.Count > 0)
                            {
                                Int32 searchTagIndex = tagList.IndexOf(search.Value.tags[0]);
                                if (searchTagIndex == 0 || tagList[searchTagIndex - 1].TextOffset < tagList[searchTagIndex].TextOffset)
                                {
                                    block.nextByProgress = search;
                                    closestDist = dist;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static LinkedListNode<TextDirectionBlock> RegisterRenderPosition(LinkedListNode<TextDirectionBlock> currentBlock, ref Single posX, Int32 vertIndex, Int32 index)
        {
            if (currentBlock == null)
                return null;
            TextDirectionBlock block = currentBlock.Value;
            if (index == block.end && block.tags.Count == 0)
            {
                block.registered = true;
                block.vertexEnd = vertIndex;
                block.width = Math.Abs(posX);
                currentBlock = block.nextByProgress;
                if (currentBlock != null)
                    currentBlock.Value.vertexStart = vertIndex;
                posX = 0f;
            }
            return currentBlock;
        }

        public static LinkedListNode<TextDirectionBlock> RegisterRenderPosition(LinkedListNode<TextDirectionBlock> currentBlock, ref Single posX, Int32 vertIndex, FFIXTextTag tag)
        {
            if (currentBlock == null)
                return null;
            TextDirectionBlock block = currentBlock.Value;
            Int32 tagIndex = block.tags.IndexOf(tag);
            if (tagIndex >= 0)
            {
                block.tags.RemoveAt(tagIndex);
                if (block.tags.Count == 0)
                {
                    block.registered = true;
                    block.vertexEnd = vertIndex;
                    block.width = Math.Abs(posX);
                    currentBlock = block.nextByProgress;
                    if (currentBlock != null)
                        currentBlock.Value.vertexStart = vertIndex;
                    posX = 0f;
                }
            }
            return currentBlock;
        }

        public static void RegisterImage(LinkedListNode<TextDirectionBlock> currentBlock, DialogImage dialogImage)
        {
            if (currentBlock != null)
                currentBlock.Value.images.Add(dialogImage);
        }

        public static Single ResolveRenderLine(LinkedList<TextDirectionBlock> list, BetterList<Vector3> verts)
        {
            Single targetX = 0f;
            Single lineWidth = 0f;
            for (LinkedListNode<TextDirectionBlock> blockNode = list.First; blockNode != null && blockNode.Value.registered; blockNode = blockNode.Next)
            {
                TextDirectionBlock block = blockNode.Value;
                if (block.isTabBlock)
                {
                    lineWidth = Math.Max(lineWidth, targetX);
                    targetX = block.width;
                }
                else
                {
                    if (!block.ltr)
                        targetX += block.width;
                    Vector3 shift = new Vector3(targetX, 0f);
                    for (Int32 i = block.vertexStart; i < block.vertexEnd; i++)
                        verts[i] += shift;
                    foreach (DialogImage dialogImage in block.images)
                        dialogImage.LocalPosition += shift;
                    if (block.ltr)
                        targetX += block.width;
                }
            }
            lineWidth = Math.Max(lineWidth, targetX);
            while (list.First != null && list.First.Value.registered)
                list.RemoveFirst();
            return lineWidth;
        }
    }
}
