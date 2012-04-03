using System;
using System.ComponentModel;


namespace MASGAU
{
	public interface IView
	{
		void attachModelItem(INotifyPropertyChanged source, String name);
		void detachModelItem();
	}
}

