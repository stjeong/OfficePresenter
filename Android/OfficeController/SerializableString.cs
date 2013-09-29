using System;
using System.Collections.Generic;
using System.Text;
using Java.Interop;

namespace OfficeController
{
    public class SerializableString : Java.Lang.Object, Java.IO.ISerializable
    {
        [Export("readObject", Throws = new[] {
        typeof (Java.IO.IOException),
        typeof (Java.Lang.ClassNotFoundException)})]
        private void ReadObjectDummy(Java.IO.ObjectInputStream source)
        {
            Console.WriteLine("I'm in ReadObject");
        }

        [Export("writeObject", Throws = new[] {
        typeof (Java.IO.IOException),
        typeof (Java.Lang.ClassNotFoundException)})]
        private void WriteObjectDummy(Java.IO.ObjectOutputStream destination)
        {
            Console.WriteLine("I'm in WriteObject");
        }

        public string Text { get; set; }
    }
}
