using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Pixed
{
    internal interface IPropertyChangeBase : INotifyPropertyChanged
    {
        void OnPropertyChanged<T>(Expression<Func<T>> property);
        void OnPropertyChanged([CallerMemberName] string? propertyName = null);
    }
    internal class PropertyChangedBase : IPropertyChangeBase
    {
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion
        #region Public Methods
        public void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            if (property.Body is MemberExpression memEx)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memEx.Member.Name));
            }
        }

        public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}