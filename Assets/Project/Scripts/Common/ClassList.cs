using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class ClassList<T> : IEnumerable<T> where T : class, new()
{
    private List<T> list = new List<T>();

    protected ClassList()
    {
    }

    protected ClassList(string _path, bool _skipFirstLine)
    {
        using (var reader = new CSVReader<T>(_path, _skipFirstLine))
        {
            list = reader.ToList();
        }
    }

    public void Add(T item)
    {
        list.Add(item);
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }

    public void Remove(T item)
    {
        list.Remove(item);
    }

    public T this[int index]
    {
        get { return this.list[index]; }
        set { this.list[index] = value; }
    }

    public int Count { get { return list.Count; } }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < list.Count; i++)
        {
            yield return list[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}