using UnityEngine;
using SimpleFileBrowser; // Import the Simple File Browser namespace

public class FileExplorerXML : MonoBehaviour
{
    public GameObject startMenu;
    public GameObject UI;

    // Method to show the file explorer when the button is clicked
    public void OpenFileExplorer()
    {
        // Set filters to only show .xml files
        FileBrowser.SetFilters(true, new FileBrowser.Filter("XML Files", ".xml"));
        FileBrowser.SetDefaultFilter(".xml");

        // Show the file load dialog
        FileBrowser.ShowLoadDialog(
            onSuccess: (string[] filePaths) => OnFilesSelected(filePaths), // Correct callback to handle an array of file paths
            onCancel: () => OnFileSelectionCancelled(), // Correct callback for cancellation
            pickMode: FileBrowser.PickMode.Files, // Only allow selecting files
            initialPath: null, // You can set a default directory here if desired
            title: "Select an XML File"
        );
    }

    // Callback when files are selected
    private void OnFilesSelected(string[] filePaths)
    {
        foreach (string filePath in filePaths)
        {
            Debug.Log("File selected: " + filePath);
            OpenXMLFile(filePath);
        }

        // Hide the StartMenu after file selection
        if (startMenu != null && UI != null)
        {
            startMenu.SetActive(false); // Hide the StartMenu UI
            UI.SetActive(true);
            Debug.Log("StartMenu hidden");
        }
    }

    // Callback when file selection is cancelled
    private void OnFileSelectionCancelled()
    {
        Debug.Log("File selection cancelled.");
    }

    // Open the selected XML file
    public void OpenXMLFile(string filePath)
    {
        try
        {
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                return;
            }

            // Open the XML file with the default application
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true // Open with the default associated program
            };
            p.Start();

            Debug.Log("Opened XML File: " + filePath);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error opening XML file: " + ex.Message);
        }
    }
}
