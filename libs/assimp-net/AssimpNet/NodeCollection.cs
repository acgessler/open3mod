using System.Collections.Generic;

namespace Assimp {
    /// <summary>
    /// A collection of child nodes owned by a parent node. Manages access to the collection while maintaing parent-child linkage.
    /// </summary>
    public sealed class NodeCollection : IList<Node> {
        private Node m_parent;
        private List<Node> m_children;

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count {
            get {
                return m_children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The child index</param>
        public Node this[int index] {
            get {
                if(index < 0 || index > Count)
                    return null;

                return m_children[index];
            }
            set {
                if(index < 0 || index > Count || value == null)
                    return;

                m_children[index] = value;
                value.SetParent(m_parent);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly {
            get { 
                return false;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="NodeCollection"/> class.
        /// </summary>
        /// <param name="parent">Parent node</param>
        internal NodeCollection(Node parent) {
            m_parent = parent;
            m_children = new List<Node>();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public void Add(Node item) {
            if(item != null) {
                m_children.Add(item);
                item.SetParent(m_parent);
            }
        }

        /// <summary>
        /// Adds a range of items to the list.
        /// </summary>
        /// <param name="items">Item array</param>
        public void AddRange(Node[] items) {
            if(items == null || items.Length == 0)
                return;

            foreach(Node child in items) {
                if(child != null) {
                    m_children.Add(child);
                    child.SetParent(m_parent);
                }
            }
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear() {
            foreach(Node node in m_children) {
                node.SetParent(null);
            }

            m_children.Clear();
        }


        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(Node item) {
            return m_children.Contains(item);
        }

        /// <summary>
        /// Copies collection contents to the array
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">Index of the array to start copying.</param>
        public void CopyTo(Node[] array, int arrayIndex) {
            m_children.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>
        /// The index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(Node item) {
            return m_children.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        public void Insert(int index, Node item) {
            if(index < 0 || index > Count || item == null)
                return;

            m_children.Insert(index, item);
            item.SetParent(m_parent);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1" /> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index) {
            Node child = this[index];

            if(child == null) {
                child.SetParent(null);
                m_children.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(Node item) {
            if(item != null && m_children.Remove(item)) {
                item.SetParent(null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Copies elements in the collection to a new array.
        /// </summary>
        /// <returns>Array of copied elements</returns>
        public Node[] ToArray() {
            return m_children.ToArray();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Node> GetEnumerator() {
            return m_children.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return m_children.GetEnumerator();
        }
    }
}
