using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ParserCSV
{
    private int _headerLineIndex = 6;
    public Dictionary<string, T> Parse<T>(string _CSVFileName) where T : new()
    {
        //딕셔너리 생성하기
        Dictionary<string, T> dictionary = new Dictionary<string, T>();
        
        //CSV 데이터 가져오기
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        if (csvData == null)
        {
            Debug.Log(_CSVFileName + " 이름의 csv file을 찾을 수 없음.");
            return dictionary;
        }
        
        //엔터를 기준으로 줄 나누기
        string[] datas = csvData.text.Split('\n');
        //헤더 값 저장하기
        string[] headers = datas[_headerLineIndex].Split(',');
        //8번째 줄부터 읽어오기(1~6번째 줄은 설명, 7번째 줄은 헤더)
        for (int i = _headerLineIndex+1; i < datas.Length; i++)
        {
            //빈 줄이라면 다음 줄로 넘어가기
            if(string.IsNullOrWhiteSpace(datas[i])) continue;
            //쉼표를 기준으로 분리하기
            string[] values = datas[i].Split(',');
            //첫번째 값은 키 값으로 사용
            string key = values[0];
            //제너릭 객체 생성
            T entry = new T();
            
            //나머지 값들을 T class 내의 필드들에 저장하기
            for (int j = 1; j < headers.Length && j < values.Length; j++)
            {
                FieldInfo field = typeof(T).GetField(headers[j], BindingFlags.Public);

                if (field != null)
                {
                    object convertedValue = ConvertValue(field.FieldType, values[j]);
                    field.SetValue(entry, convertedValue);
                }
            }
            
            //딕셔너리에 추가하기
            dictionary[key] = entry;
        }
        return dictionary;
    }
    
    public Dictionary<string, DialogueData> ParseDialogue(string _CSVFileName)
    {
        //딕셔너리 생성하기
        Dictionary<string, DialogueData> dictionary = new Dictionary<string, DialogueData>();
        
        //CSV 데이터 가져오기
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        if (csvData == null)
        {
            Debug.Log(_CSVFileName + " 이름의 csv file을 찾을 수 없음.");
            return dictionary;
        }
        
        //엔터를 기준으로 줄 나누기
        string[] datas = csvData.text.Split('\n');
        //헤더 값 저장하기
        string[] headers = datas[_headerLineIndex].Split(',');
        
        //8번째 줄부터 읽어오기(1~6번째 줄은 설명, 7번째 줄은 헤더)
        for (int i = _headerLineIndex+1; i < datas.Length; i++)
        {
            //쉼표를 기준으로 분리하기
            string[] values = datas[i].Split(',');
            //첫번째 값은 키 값으로 사용
            string key = values[0];
            // Debug.Log($"key : {key}");
            //TextData 리스트 생성
            DialogueData dialogueData = new DialogueData();
            List<DialogueTextData> textDatas = new List<DialogueTextData>();
            //캐릭터 id 값 저장
            dialogueData.characterId = values[1];
            // Debug.Log($"characterID : {values[1]}");
            DialogueTextData textData = new DialogueTextData();
            
            //dialogue text 저장
            string newStr = "";
            for (int j = 0; j < values[2].Length; j++)
            {
                if (values[2][j] == '@')
                {
                    newStr += ',';
                }
                else
                {
                    newStr += values[2][j];
                }
            }
            textData.dialogueText = newStr;
            textData.choiceText1 = values[3];
            textData.choiceText2 = values[4];
            textData.resultDialogueId1 = values[5];
            textData.resultDialogueId2 = values[6];
            textData.nextDialougeId = values[7];
            
            textDatas.Add(textData);

            // Debug.Log($"dialogueText : {textData.dialogueText}");
            // Debug.Log($"choiceText1 : {textData.choiceText1}");
            // Debug.Log($"choiceText2 : {textData.choiceText2}");
            // Debug.Log($"resultDialogueId1 : {textData.resultDialogueId1}");
            // Debug.Log($"resultDialogueId2 : {textData.resultDialogueId2}");
            // Debug.Log($"nextDialougeId : {textData.nextDialougeId}");
            
            for (int j = i + 1; j < datas.Length; j++)
            {
                string[] _values = datas[j].Split(',');
                if (string.IsNullOrEmpty(_values[0]) && string.IsNullOrEmpty(_values[1]))
                {
                    DialogueTextData _textData = new DialogueTextData();
                    //dialogue text 저장
                    string _newStr = "";
                    for (int k = 0; k < _values[2].Length; k++)
                    {
                        if (_values[2][k] == '@')
                        {
                            _newStr += ',';
                        }
                        else
                        {
                            _newStr += _values[2][k];
                        }
                    }
                    _textData.dialogueText = _newStr;
                    _textData.choiceText1 = _values[3];
                    _textData.choiceText2 = _values[4];
                    _textData.resultDialogueId1 = _values[5];
                    _textData.resultDialogueId2 = _values[6];
                    _textData.nextDialougeId = _values[7];
                    // Debug.Log($"dialogueText : {_textData.dialogueText}");
                    // Debug.Log($"choiceText1 : {_textData.choiceText1}");
                    // Debug.Log($"choiceText2 : {_textData.choiceText2}");
                    // Debug.Log($"resultDialogueId1 : {_textData.resultDialogueId1}");
                    // Debug.Log($"resultDialogueId2 : {_textData.resultDialogueId2}");
                    // Debug.Log($"nextDialougeId : {_textData.nextDialougeId}");
                    textDatas.Add(_textData);
                }
                else
                {
                    i = j-1;
                    //딕셔너리에 추가하기
                    dialogueData.textDatas = textDatas.ToArray();
                    dictionary[key] = dialogueData;
                    break;
                }
            }
        }
        return dictionary;
    }
    
    private object ConvertValue(System.Type fieldType, string value)
    {
        if (fieldType == typeof(int)) return int.Parse(value);
        if (fieldType == typeof(float)) return float.Parse(value);
        if (fieldType == typeof(bool)) return bool.Parse(value);
        return value;
    }
}
