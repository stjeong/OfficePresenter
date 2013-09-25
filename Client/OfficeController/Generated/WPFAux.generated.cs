





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
    using System.Windows.Media.Imaging;
    
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
    
    [DataContract]
    public partial class PPTController
    			: INotifyPropertyChanged
    					
    {
    	public class Properties
    	{
    		public const string Slides = " Slides";
    		public const string Selectedtem = " Selectedtem";
    	}
    	
    	ObservableCollection<SlidePage> _slides;
    	/// <exclude />
    	[DataMember]
    	public ObservableCollection<SlidePage> Slides
    	{
    		get { return this._slides; }
    		
    		set
    		{
    			if (this._slides == value)
    			{
    				return;
    			}
    			
    			this._slides = value;
    			OnPropertyChanged("Slides"); 
    		}
    	}
    	
    	SlidePage _selectedtem;
    	/// <exclude />
    	[DataMember]
    	public SlidePage Selectedtem
    	{
    		get { return this._selectedtem; }
    		
    		set
    		{
    			if (this._selectedtem == value)
    			{
    				return;
    			}
    			
    			this._selectedtem = value;
    			OnPropertyChanged("Selectedtem"); 
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
    public partial class SlidePage
    			: INotifyPropertyChanged
    					
    {
    	public class Properties
    	{
    		public const string Image = " Image";
    		public const string Memo = " Memo";
    		public const string TagData = " TagData";
    		public const string IsSelected = " IsSelected";
    		public const string AnimationRemains = " AnimationRemains";
    	}
    	
    	BitmapImage _image;
    	/// <exclude />
    	[DataMember]
    	public BitmapImage Image
    	{
    		get { return this._image; }
    		
    		set
    		{
    			if (this._image == value)
    			{
    				return;
    			}
    			
    			this._image = value;
    			OnPropertyChanged("Image"); 
    		}
    	}
    	
    	string _memo;
    	/// <exclude />
    	[DataMember]
    	public string Memo
    	{
    		get { return this._memo; }
    		
    		set
    		{
    			if (this._memo == value)
    			{
    				return;
    			}
    			
    			this._memo = value;
    			OnPropertyChanged("Memo"); 
    		}
    	}
    	
    	SlideItemData _tagData;
    	/// <exclude />
    	[DataMember]
    	public SlideItemData TagData
    	{
    		get { return this._tagData; }
    		
    		set
    		{
    			if (this._tagData == value)
    			{
    				return;
    			}
    			
    			this._tagData = value;
    			OnPropertyChanged("TagData"); 
    		}
    	}
    	
    	bool _isSelected;
    	/// <exclude />
    	[DataMember]
    	public bool IsSelected
    	{
    		get { return this._isSelected; }
    		
    		set
    		{
    			if (this._isSelected == value)
    			{
    				return;
    			}
    			
    			this._isSelected = value;
    			OnPropertyChanged("IsSelected"); 
    		}
    	}
    	
    	int _animationRemains;
    	/// <exclude />
    	[DataMember]
    	public int AnimationRemains
    	{
    		get { return this._animationRemains; }
    		
    		set
    		{
    			if (this._animationRemains == value)
    			{
    				return;
    			}
    			
    			this._animationRemains = value;
    			OnPropertyChanged("AnimationRemains"); 
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

