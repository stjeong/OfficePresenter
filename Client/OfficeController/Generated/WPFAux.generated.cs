





namespace OfficeController
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
    public partial class App
    			: INotifyPropertyChanged
    					
    {
    	public class Properties
    	{
    		public const string IPList = " IPList";
    		public const string IPSelected = " IPSelected";
    		public const string Port = " Port";
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
    	
    	string _iPSelected;
    	/// <exclude />
    	[DataMember]
    	public string IPSelected
    	{
    		get { return this._iPSelected; }
    		
    		set
    		{
    			if (this._iPSelected == value)
    			{
    				return;
    			}
    			
    			this._iPSelected = value;
    			OnPropertyChanged("IPSelected"); 
    		}
    	}
    	
    	int _port;
    	/// <exclude />
    	[DataMember]
    	public int Port
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
    
    [DataContract]
    public partial class MainPage
    			: INotifyPropertyChanged
    					
    {
    	public class Properties
    	{
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

