using System.Collections;

public interface ICutInObject
{
	string ID { get; }

	void StartData(CutInData cutInData);

	void Play();

	void Stop();

	void EnterStatus();

	void UpdateStatus();

	void ExitStatus();

	IEnumerator CutInUpdate();
}
