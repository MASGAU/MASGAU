using System;

namespace MASGAU
{
	public interface IView
	{
		void attachModelItem(AModelItem source, String name);
		void detachModelItem();
	}
}

