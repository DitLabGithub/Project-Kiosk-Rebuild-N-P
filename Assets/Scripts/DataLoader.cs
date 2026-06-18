using UnityEngine;
using System.Linq;

public class DataLoader : MonoBehaviour
{
    private readonly string[] npcVariables =
    {
        "friendship",
        "UpsoldNumber"
    };

    public NPC LoadNPC(string fileName)
    {
        TextAsset jsonFile =
            Resources.Load<TextAsset>(
                fileName
            );

        if (jsonFile == null)
        {
            Debug.LogError(
                "JSON not found: "
                + fileName
            );

            return null;
        }

        CharacterData data =
            JsonUtility.FromJson<CharacterData>(
                jsonFile.text
            );

        // Main avatar
        data.avatar =
            Resources.Load<Sprite>(
                data.avatarPath
            );

        if (data.avatar == null)
        {
            Debug.LogWarning(
                "Avatar missing: "
                + data.avatarPath
            );
        }

        // Alternate SSI avatars
        if (data.ssiProfiles != null)
        {
            foreach (
                CustomSSIEntry profile
                in data.ssiProfiles
            )
            {
                if (
                    profile.data != null
                    && !string.IsNullOrEmpty(
                        profile.data.avatarPath
                    )
                )
                {
                    profile.data.avatar =
                        Resources.Load<Sprite>(
                            profile.data.avatarPath
                        );

                    if (
                        profile.data.avatar
                        == null
                    )
                    {
                        Debug.LogWarning(
                            "SSI avatar missing: "
                            + profile.data.avatarPath
                        );
                    }
                }
            }
        }

        NPC npc =
            new NPC(data);

        // Variables
        foreach (
            VariableData variable
            in data.variables
        )
        {
            bool belongsToNPC =
                npcVariables.Contains(
                    variable.name
                );

            if (belongsToNPC)
            {
                npc.SetVariable(
                    variable.name,
                    variable.value
                );
            }
            else
            {
                if (
                    string.IsNullOrEmpty(
                        DialogueManager.Instance
                            .GetVariable(
                                variable.name
                            )
                    )
                )
                {
                    DialogueManager.Instance
                        .SetVariable(
                            variable.name,
                            variable.value
                        );
                }
            }
        }

        return npc;
    }
}