using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FPSControl.Data
{
    [System.Serializable]
    public class FalloffData : object, IEnumerable
    {
        public float distance;
        [SerializeField]
        List<FalloffPoint> _points = new List<FalloffPoint>();

        public FalloffData() { }

        public FalloffPoint[] ToArray()
        {
            return _points.ToArray();
        }

        public int Length { get { return _points.Count; } }

        public static implicit operator AnimationCurve(FalloffData data)
        {
            AnimationCurve ac = new AnimationCurve();

            List<FalloffPoint> points = new List<FalloffPoint>();

            int length = data.Length;
            if (length == 0)
            {
                points.Add( new FalloffPoint(0, 1));
                length++;
                if (length == 1)
                {
                    points.Add(new FalloffPoint(1, 0));
                    length++;
                }
            }
            else if (length == 1)
            {
                if (data[0].location != 0) points.Add(new FalloffPoint(0, 1));
                points.Add(data[0]);
                points.Add(new FalloffPoint(1, 0));
            }
            else
            {
                points = data._points;
                if (data[0].location != 0) points.Insert(0, new FalloffPoint(0, 1));
                if (data[data.Length-1].location != 1) points.Add(new FalloffPoint(1, 0));
            }

            for (int i = 0; i < points.Count; i++)
            {
                ac.AddKey(points[i].location, points[i].value);
            }

            return ac;
        }

        public float Evaluate(float location)
        {
            AnimationCurve ac = (AnimationCurve) this;
            return ac.Evaluate(location);
        }

        public FalloffPoint this[int index]
        {
            get
            {
                return _points[index];
            }
            set
            {
                _points[index] = value;
                Reorder();
            }
        }

        public int this[FalloffPoint point]
        {
            get
            {
                return IndexOf(point);
            }
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _points.Count; i++)
            {
                yield return _points[i];
            }
        }

        public int IndexOf(FalloffPoint point)
        {
            return _points.IndexOf(point);
        }

        public void Add(FalloffPoint point)
        {
            _points.Add(point);
            Reorder();
        }

        public void Remove(FalloffPoint point)
        {
            _points.Remove(point);
            Reorder();
        }

        public void RemoveAt(int index)
        {
            _points.RemoveAt(index);
            Reorder();
        }

        void Reorder()
        {
            _points = _points.OrderBy(point => point.location).ToList();
        }

    }

    [System.Serializable]
    public class FalloffPoint : object, System.IComparable
    {
        //float _location;
        public float location;// { get { return _location; } set { _location = value; } }
        //float _value;
        public float value;// { get { return _value; } set { _value = value; } }

        public FalloffPoint(float location, float value)
        {
            this.location = Mathf.Clamp(location, 0F, 1F);
            this.value = Mathf.Clamp(value, 0F, 1F);
        }

        public int CompareTo(object other)
        {
            FalloffPoint p = (FalloffPoint)other;
            if (p.location < location) return -1;
            else if (p.location > location) return 1;
            else if (p.location == location) return 0;

            return 0;
        }

        public static implicit operator Vector2(FalloffPoint p)
        {
            return new Vector2(p.location, p.value);
        }

        public static implicit operator bool(FalloffPoint p)
        {
            return p != null;
        }
    }
}
