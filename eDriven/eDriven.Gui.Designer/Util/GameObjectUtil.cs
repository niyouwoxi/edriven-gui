﻿#region License

/*
 
Copyright (c) 2010-2014 Danko Kozar

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
 
*/

#endregion License

using System;
using UnityEngine;

namespace eDriven.Gui.Designer.Util
{
    public static class GameObjectUtil
    {
        /// <summary>
        /// Creates a game object at path
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static GameObject CreateGameObjectAtPath(string s)
        {
            string[] parts = s.Split(new[] { "/" }, StringSplitOptions.None);

            GameObject go = null;
            GameObject prevGo = null;

            string path = string.Empty;
            foreach (string part in parts)
            {
                path += string.Format("/{0}", part);
                go = GameObject.Find(path);
                if (null == go)
                {
                    go = new GameObject(part);
                    if (null != prevGo)
                        go.transform.parent = prevGo.transform;
                }
                prevGo = go;
            }

            return go;
        }
    }
}