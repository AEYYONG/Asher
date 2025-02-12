using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParserTest : MonoBehaviour
{
    private Dictionary<string, DialogueData> dialogueDatas = new Dictionary<string, DialogueData>();
    // Start is called before the first frame update
    void Start()
    {
        ParserCSV parser = new ParserCSV();
        dialogueDatas = parser.ParseDialogue("NoaClearDialogue");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
