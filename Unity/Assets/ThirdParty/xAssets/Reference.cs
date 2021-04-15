//
// Reference.cs
//
// Author:
//       MoMo的奶爸 <xasset@qq.com>
//
// Copyright (c) 2020 MoMo的奶爸
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation bundles (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

namespace libx
{
    public class Reference
    {
        public string name { get; set; }

        private List<Object> _requires = null;

        public bool IsUnused()
        {
            return refCount <= 0;
        }

        public void UpdateRequires()
        {
            if (_requires == null)
            {
                return;
            }
            for (var i = 0; i < _requires.Count; i++)
            {
                var item = _requires[i];
                if (item != null)
                    break;
                _requires.RemoveAt(i);
                i--;
            }
            if (_requires.Count == 0)
            {
                Release();
                _requires = null;
            }
        }

        public int refCount { get; private set; }

        public virtual void Retain()
        {
            refCount++;
        }

        public virtual void Release()
        {
            refCount--;
            if (refCount < 0)
            {
                Debug.LogErrorFormat("Release: {0} refCount < 0", name);
            } 
        }

        public void Require(Object obj)
        {
            if (refCount > 0)
            {
                Release();
            }
            if (_requires == null)
            {
                _requires = new List<Object>();
                Retain();
            }
            _requires.Add(obj);
        }

        public void Dequire(Object obj)
        {
            if (_requires == null)
            {
                return;
            }
            _requires.Remove(obj);
        }
    }
}
