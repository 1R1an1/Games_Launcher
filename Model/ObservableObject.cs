using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Games_Launcher.Model
{
	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		/// Método helper para asignar un valor a un campo privado y disparar el evento PropertyChanged automáticamente.
		/// Esto evita repetir siempre la misma estructura de setter con OnPropertyChanged.
		/// Uso típico:
		/// private bool _isRunning;
		/// public bool IsRunning
		/// {
		///     get => _isRunning;
		///     set => SetProperty(ref _isRunning, value);
		/// }
		/// </summary>
		/// <typeparam name="T">Tipo de la propiedad</typeparam>
		/// <param name="field">Referencia al campo privado que guarda el valor</param>
		/// <param name="value">Nuevo valor a asignar</param>
		/// <param name="propertyName">Nombre de la propiedad (opcional, lo llena CallerMemberName automáticamente)</param>
		/// <returns>Devuelve true si el valor cambió, false si era igual</returns>
		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(field, value))
				return false;

			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
