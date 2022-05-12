using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Main : MonoBehaviour
{
    [SerializeField] public InputField nameInput;
    [SerializeField] public Text infoText;
    [SerializeField] public AtomFactory atomFactory;
    [SerializeField] public GameObject temp;
    [SerializeField] public float rotationSpeed = 20;
    [SerializeField] public Camera cam;

    private Molecule _currentMolecule;
    public Molecule CurrentMolecule
    {
        get { return _currentMolecule; }
        set
        {
            _currentMolecule = value;
            if(_currentMolecule != null) AlignMolecule(_currentMolecule);
        }
    }

    private NameParser nameParser;
    void Start()
    {
        nameParser = new NameParser(this,atomFactory); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            nameParser.parseAndCreate(nameInput.text);
        }
        if(Input.GetMouseButton(0) && CurrentMolecule != null)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;

            Vector3 right = Vector3.Cross(cam.transform.up, CurrentMolecule.Position - cam.transform.position);
            Vector3 up = Vector3.Cross(CurrentMolecule.Position - cam.transform.position, right);
            CurrentMolecule.Rotation = Quaternion.AngleAxis(-rotX, up) * CurrentMolecule.Rotation;
            CurrentMolecule.Rotation = Quaternion.AngleAxis(rotY, right) * CurrentMolecule.Rotation;
        }
    }

    public void setInfoText(string text)
    {
        infoText.text = text;
    }

    public void destroyPrevMolecule()
    {
        Destroy(GameObject.Find("Molecule"));
        CurrentMolecule = null;
    }

    public void AlignMolecule(Molecule molecule)
    {
        List<MoleculeTreeNode> nodes = CurrentMolecule.rootTreeNode.GetAllNodesWithAtom<CarbonAtom>();
        float count = nodes.Count;
        Vector3 total = nodes.Aggregate<MoleculeTreeNode, Vector3>(new Vector3(0f, 0f, 0f), (sum, node) =>
           {
               sum += node.NodeStructure.Position;
               return sum;
           });
        Vector3 shift = total / count;
        List<MoleculeTreeNode> allNodes = CurrentMolecule.rootTreeNode.GetAllNodes();
        foreach (MoleculeTreeNode node in allNodes)
        {
            node.NodeStructure.Position -= shift;
        }
        CurrentMolecule.Rotation = Quaternion.FromToRotation(shift, Vector3.right);
    }
}