





namespace DocumentController
{
    using System.Data;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.IO;
    using System.Diagnostics;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    
    [DataContract]
    public partial class MainWindow
    			: INotifyPropertyChanged
    					
    {
    	public class Properties
    	{
    		public const string FilePath = " FilePath";
    		public const string Port = " Port";
    		public const string IPList = " IPList";
    		public const string Ready = " Ready";
    	}
    	
    	string _filePath;
    	/// <exclude />
    	[DataMember]
    	public string FilePath
    	{
    		get { return this._filePath; }
    		
    		set
    		{
    			if (this._filePath == value)
    			{
    				return;
    			}
    			
    			this._filePath = value;
    			OnPropertyChanged("FilePath"); 
    		}
    	}
    	
    	short _port;
    	/// <exclude />
    	[DataMember]
    	public short Port
    	{
    		get { return this._port; }
    		
    		set
    		{
    			if (this._port == value)
    			{
    				return;
    			}
    			
    			this._port = value;
    			OnPropertyChanged("Port"); 
    		}
    	}
    	
    	ObservableCollection<string> _iPList;
    	/// <exclude />
    	[DataMember]
    	public ObservableCollection<string> IPList
    	{
    		get { return this._iPList; }
    		
    		set
    		{
    			if (this._iPList == value)
    			{
    				return;
    			}
    			
    			this._iPList = value;
    			OnPropertyChanged("IPList"); 
    		}
    	}
    	
    	string _ready;
    	/// <exclude />
    	[DataMember]
    	public string Ready
    	{
    		get { return this._ready; }
    		
    		set
    		{
    			if (this._ready == value)
    			{
    				return;
    			}
    			
    			this._ready = value;
    			OnPropertyChanged("Ready"); 
    		}
    	}
    	
    	
    	public event PropertyChangedEventHandler PropertyChanged;
    	protected virtual void OnPropertyChanged(string propertyName)
    	{
    		if (PropertyChanged == null)
    		{
    			return;
    		}
    
    		PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    	}    
    }

}

