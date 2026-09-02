using System.Collections;
using NPOI.POIFS.FileSystem;

namespace NPOI.POIFS.EventFileSystem;

public class POIFSReaderRegistry
{
	private ArrayList omnivorousListeners;

	private Hashtable selectiveListeners;

	private Hashtable chosenDocumentDescriptors;

	public POIFSReaderRegistry()
	{
		omnivorousListeners = new ArrayList();
		selectiveListeners = new Hashtable();
		chosenDocumentDescriptors = new Hashtable();
	}

	public void RegisterListener(POIFSReaderListener listener, POIFSDocumentPath path, string documentName)
	{
		if (omnivorousListeners.Contains(listener))
		{
			return;
		}
		ArrayList arrayList = (ArrayList)selectiveListeners[listener];
		if (arrayList == null)
		{
			arrayList = new ArrayList();
			selectiveListeners[listener] = arrayList;
		}
		DocumentDescriptor documentDescriptor = new DocumentDescriptor(path, documentName);
		if (arrayList.Add(documentDescriptor) >= 0)
		{
			ArrayList arrayList2 = (ArrayList)chosenDocumentDescriptors[documentDescriptor];
			if (arrayList2 == null)
			{
				arrayList2 = new ArrayList();
				chosenDocumentDescriptors[documentDescriptor] = arrayList2;
			}
			arrayList2.Add(listener);
		}
	}

	public void RegisterListener(POIFSReaderListener listener)
	{
		if (!omnivorousListeners.Contains(listener))
		{
			RemoveSelectiveListener(listener);
			omnivorousListeners.Add(listener);
		}
	}

	public IEnumerator GetListeners(POIFSDocumentPath path, string name)
	{
		ArrayList arrayList = new ArrayList(omnivorousListeners);
		ArrayList arrayList2 = (ArrayList)chosenDocumentDescriptors[new DocumentDescriptor(path, name)];
		if (arrayList2 != null)
		{
			arrayList.AddRange(arrayList2);
		}
		return arrayList.GetEnumerator();
	}

	private void RemoveSelectiveListener(POIFSReaderListener listener)
	{
		ArrayList arrayList = (ArrayList)selectiveListeners[listener];
		if (arrayList != null)
		{
			selectiveListeners.Remove(listener);
			IEnumerator enumerator = arrayList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DropDocument(listener, (DocumentDescriptor)enumerator.Current);
			}
		}
	}

	private void DropDocument(POIFSReaderListener listener, DocumentDescriptor descriptor)
	{
		ArrayList obj = (ArrayList)chosenDocumentDescriptors[descriptor];
		obj.Remove(listener);
		if (obj.Count == 0)
		{
			chosenDocumentDescriptors.Remove(descriptor);
		}
	}
}
