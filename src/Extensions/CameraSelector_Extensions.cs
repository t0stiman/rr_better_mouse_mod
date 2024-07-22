namespace better_mouse_mod.Extensions;

public static class CameraSelector_Extensions
{
	/// <summary>
	/// same as CameraSelector.JumpToSeat but doesn't switch to first person camera
	/// </summary>
	public static void JumpToSeatWithoutCameraChange(this CameraSelector deez)
	{
		var selectedCar = TrainController.Shared.SelectedCar;

		if (selectedCar == null)
		{
			Main.Error($"{nameof(JumpToSeatWithoutCameraChange)}: No selected car.");
			return;
		}

		var bestSeat = deez.FindBestSeat(selectedCar, deez.character.character.Seat);
		if (bestSeat == null) // no unoccupied seats
		{
			deez.character.JumpToCar(selectedCar);
		}
		else // yes unoccupied seat(s)
		{
			deez.character.JumpTo(bestSeat.FootPosition, bestSeat.transform.rotation);
			deez.character.Sit(bestSeat);
		}
	}
}