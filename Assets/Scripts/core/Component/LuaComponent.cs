using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

public class LuaComponent : MonoBehaviour
{
    LuaFunction destroyfunc = null;
    
    public LuaTable LuaModule
    {
        get;
        private set;
    }

    public static LuaTable GetLuaComponent(GameObject go)
    {
        var luaCom = go.GetComponent<LuaComponent>();
        if (null == luaCom)
        {
            return null;
        }
        return luaCom.LuaModule;
    }

    public static LuaComponent Bind(GameObject obj, LuaTable tb)
    {
        LuaComponent luaComp = obj.GetComponent<LuaComponent>();
        if (luaComp == null)
        {
            luaComp = obj.AddComponent<LuaComponent>();
        }
        luaComp.SetLuaTable(tb);
        return luaComp;
    }

    public void Cleanup()
    {
        if (LuaModule != null)
        {
            if (destroyfunc != null)
            {
                destroyfunc.Call(LuaModule);
            }
            LuaModule.Dispose();
            LuaModule = null;
        }
    }

    public void SetLuaTable(LuaTable tb)
    {
        Cleanup();
        LuaModule = tb;
        destroyfunc = tb.GetLuaFunction("OnDestroy");
    }

    public LuaTable GetLuaTable()
    {
        return LuaModule;
    }

    /*
    protected void Start()
    {
        LuaFunction func = LuaModule.GetLuaFunction("Start");
        if (func != null)
            func.Call(LuaModule);
    }*/

    // MonoBehaviour callback
    protected void OnDestroy()
    {
        Cleanup();
    }

    void OnControllerColliderHit(ControllerColliderHit target)
    {
        LuaFunction func = LuaModule.GetLuaFunction("OnControllerColliderHit");
        if (func != null)
            func.Call<LuaTable, ControllerColliderHit>(LuaModule, target);
    }

    void OnTriggerEnter(Collider target)
    {
        LuaFunction func = LuaModule.GetLuaFunction("OnTriggerEnter");
        if (func != null)
            func.Call<LuaTable, Collider>(LuaModule, target);

    }

    void OnTriggerExit(Collider target)
    {
        LuaFunction func = LuaModule.GetLuaFunction("OnTriggerExit");
        if (func != null)
            func.Call<LuaTable, Collider>(LuaModule, target);

    }

    /// <summary>
    /// 辅助功能
    /// </summary>
    /// 
    bool attackFlagVisible = false;
    int attackFlagRange;
    Vector3 attackFlagPosition;
    Vector3 attackFlagDirection;
    float attackFlagAngle;
    float attackFlagRadius;
    float attackFlagLong;
    float attackFlagWide;
    Color attackFlagColor;

    public void ShowAttackFlagSector(Vector3 position, Vector3 direction, float radius, float angle, Color color)
    {
        attackFlagRange = 0;
        attackFlagVisible = true;
        attackFlagPosition = position;
        attackFlagDirection = direction;
        attackFlagAngle = angle;
        attackFlagRadius = radius;
        attackFlagColor = color;
        attackFlagColor.a = 0.5f;
    }

    public void ShowAttackFlagRectangle(Vector3 position, Vector3 direction, float longs, float wide, Color color)
    {
        attackFlagRange = 1;
        attackFlagVisible = true;
        attackFlagPosition = position;
        attackFlagDirection = direction;
        attackFlagLong = longs;
        attackFlagWide = wide;
        attackFlagColor = color;
        attackFlagColor.a = 0.5f;
    }

    public void ClearAttackFlag()
    {
        attackFlagVisible = false;
    }

    bool attackRangeVisible = false;
    Vector3 attackRangePosition;
    float attackRangeRadius;
    Color attackRangeColor;

    public void ShowAttackRange(Vector3 position, float radius, Color color)
    {
        attackRangeVisible = true;
        attackRangePosition = position;
        attackRangeRadius = radius;
        attackRangeColor = color;
        attackRangeColor.a = 0.5f;
    }

    public void ClearAttackRange()
    {
        attackRangeVisible = false;
    }

    bool attackRingVisible = false;
    Vector3 attackRingPosition;
    float attackRingMinRadius;
    float attackRingMaxRadius;
    Color attackRingMinColor;
    Color attackRingMaxColor;

    public void ShowAttackRing(Vector3 position, float minRadius, float maxRadius, Color color1, Color color2)
    {
        attackRingVisible = true;
        attackRingPosition = position;
        attackRingMinRadius = minRadius;
        attackRingMaxRadius = maxRadius;
        attackRingMinColor = color1;
        attackRingMinColor.a = 0.5f;
        attackRingMaxColor = color2;
        attackRingMaxColor.a = 0.5f;
    }

    public void ClearAttackRing()
    {
        attackRingVisible = false;
    }


    bool attackRayVisible = false;
    Vector3 attackRayOrigin;
    Vector3 attackRayDirec;
    Color attackRayColor;

    public void ShowAttackRay(Vector3 start, Vector3 direc, Color color)
    {
        attackRayVisible = true;
        attackRayOrigin = start;
        attackRayDirec = direc;
        attackRayColor = color;
        attackRayColor.a = 0.5f;
    }
    public void ClearAttackRay()
    {
        attackRayVisible = false;
    }

    bool attackBoxVisible = false;
    Vector3 attackBoxStart;
    Vector3 attackBoxSize;
    Vector3 attackBoxDirection;
    Color attackBoxColor;

    public void ShowAttackBox(Vector3 start, Vector3 size, Vector3 direc, Color color)
    {
        attackBoxVisible = true;
        attackBoxStart = start;
        attackBoxSize = size;
        attackBoxDirection = direc;
        attackBoxColor = color;
        attackBoxColor.a = 0.5f;
    }
    public void ClearAttackBox()
    {
        attackBoxVisible = false;
    }

    bool attackSectorVisible = false;
    Vector3 attackSectorOrigin;
    Vector3 attackSectorDirection;
    float attackSectorRadius;
    float attackSectorAngle;
    Color attackSectorColor;

    public void ShowAttackSector(Vector3 origin, Vector3 direc, float radius, float angle, Color color)
    {
        attackSectorVisible = true;
        attackSectorOrigin = origin;
        attackSectorDirection = direc;
        attackSectorRadius = radius;
        attackSectorAngle = angle;
        attackSectorColor.a = 0.5f;
    }
    public void ClearAttackSector()
    {
        attackSectorVisible = false;
    }

    private void OnDrawGizmos()
    {
        if (attackFlagVisible)
        {
            Gizmos.color = attackFlagColor;
            if (attackFlagRange == 0)
            {
                DrawSector(attackFlagPosition, attackFlagDirection, attackFlagRadius, attackFlagAngle, Vector3.up);
            }
            else if(attackFlagRange == 1)
            {
                DrawRectangle(attackFlagPosition, attackFlagDirection, attackFlagLong, attackFlagWide, Vector3.up);
            }
        }
        if (attackRangeVisible)
        {
            Gizmos.color = attackRangeColor;

            DrawRound(attackRangePosition, attackRangeRadius);
        }
        if (attackRayVisible)
        {
            Gizmos.color = attackRayColor;
            Gizmos.DrawRay(attackRayOrigin, attackRayDirec);
        }
        if (attackBoxVisible)
        {
            Gizmos.color = attackBoxColor;
            DrawBox(attackBoxStart, attackBoxSize, attackBoxDirection);
        }
        if (attackSectorVisible)
        {
            Gizmos.color = attackSectorColor;
            DrawSector(attackSectorOrigin, attackSectorDirection, attackSectorRadius, attackSectorAngle, Vector3.up);
        }
        if (attackRingVisible)
        {
            Gizmos.color = attackRingMinColor;
            DrawRound(attackRingPosition, attackRingMinRadius);
            Gizmos.color = attackRingMaxColor;
            DrawRound(attackRingPosition, attackRingMaxRadius);
        }
    }

    void DrawRound(Vector3 origin, float radius)
    {
        Mesh mesh = GetMeshRound(radius, 20);

        Gizmos.DrawMesh(mesh, origin);
    }

    public static Mesh GetMeshRound(float radio, int pointAmount)
    {
        float eachAngle = 360f / pointAmount;
        List<Vector3> vertices = new List<Vector3>();
        for (int i = 0; i <= pointAmount; i++)
        {
            Vector3 pos = Quaternion.Euler(0f, eachAngle * i, 0f) * Vector3.forward * radio;
            vertices.Add(pos);
        }
        int[] triangles;
        Mesh mesh = new Mesh();

        int trangleAmount = vertices.Count - 2;
        triangles = new int[3 * trangleAmount];

        for (int i = 0; i < trangleAmount; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    void DrawRectangle(Vector3 origin, Vector3 direction, float longs, float wide, Vector3 axis)
    {
        Vector3[] vectors = new Vector3[4];
        vectors[0] = Quaternion.AngleAxis(-90, axis) * direction * wide / 2;
        vectors[1] = vectors[0] + direction * longs;
        vectors[3] = Quaternion.AngleAxis(90, axis) * direction * wide / 2;
        vectors[2] = vectors[3] + direction * longs;

        Gizmos.DrawLine(vectors[0] + origin, vectors[1] + origin);
        Gizmos.DrawLine(vectors[1] + origin, vectors[2] + origin);
        Gizmos.DrawLine(vectors[2] + origin, vectors[3] + origin);
        Gizmos.DrawLine(vectors[3] + origin, vectors[0] + origin);

        Gizmos.DrawMesh(GetRectangleMesh(vectors), origin);
    }

    public static Mesh GetRectangleMesh(Vector3[] vectors)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vectors;
        mesh.triangles = new int[]
        { 0, 1, 2,
          0, 2, 3
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    void DrawBox(Vector3 start, Vector3 size, Vector3 direc)
    {
        Mesh mesh = GetBoxMesh(size);
        Gizmos.DrawMesh(mesh, start, Quaternion.LookRotation(direc, Vector3.up));
    }

    Mesh GetBoxMesh(Vector3 size)
    {
        int vertices_count = 4 * 6;
        Vector3[] vertices = new Vector3[vertices_count];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(0, size.y, 0);
        vertices[2] = new Vector3(size.x, 0, 0);
        vertices[3] = new Vector3(size.x, size.y, 0);

        vertices[4] = new Vector3(size.x, 0, size.z);
        vertices[5] = new Vector3(size.x, size.y, size.z);
        vertices[6] = new Vector3(0, 0, size.z);
        vertices[7] = new Vector3(0, size.y, size.z);

        vertices[8] = vertices[6];
        vertices[9] = vertices[7];
        vertices[10] = vertices[0];
        vertices[11] = vertices[1];
        vertices[12] = vertices[2];
        vertices[13] = vertices[3];
        vertices[14] = vertices[4];
        vertices[15] = vertices[5];

        vertices[16] = vertices[1];
        vertices[17] = vertices[7];
        vertices[18] = vertices[3];
        vertices[19] = vertices[5];
        
        vertices[20] = vertices[2];
        vertices[21] = vertices[4];
        vertices[22] = vertices[0];
        vertices[23] = vertices[6];


        //triangles(索引三角形、必须):
        int triangles_count = 6 * 2 * 3;                  //索引三角形的索引点个数
        int[] triangles = new int[triangles_count];            //索引三角形数组
        for (int i = 0, vi = 0; i < triangles_count; i += 6, vi += 4)
        {
            triangles[i] = vi;
            triangles[i + 1] = vi + 1;
            triangles[i + 2] = vi + 2;
            triangles[i + 3] = vi + 3;
            triangles[i + 4] = vi + 2;
            triangles[i + 5] = vi + 1;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    void DrawSector(Vector3 origin, Vector3 direction, float radius, float angle, Vector3 axis)
    {
        Vector3 leftdir = Quaternion.AngleAxis(-angle / 2, axis) * direction;
        Vector3 rightdir = Quaternion.AngleAxis(angle / 2, axis) * direction;

        Vector3 currentP = origin + leftdir * radius;
        Vector3 oldP;
        if (angle != 360)
        {
            Gizmos.DrawLine(origin, currentP);
        }
        for (int i = 0; i < angle / 10; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * leftdir;
            oldP = currentP;
            currentP = origin + dir * radius;
            Gizmos.DrawLine(oldP, currentP);
        }
        oldP = currentP;
        currentP = origin + rightdir * radius;
        Gizmos.DrawLine(oldP, currentP);
        if (angle != 360)
        {
            Gizmos.DrawLine(currentP, origin);
        }

        Gizmos.DrawMesh(GetSectorMesh(radius, angle, axis), origin, Quaternion.LookRotation(direction, axis));
    }


    public static Mesh GetSectorMesh(float radius, float angle, Vector3 axis)
    {
        Vector3 leftdir = Quaternion.AngleAxis(-angle / 2, axis) * Vector3.forward;
        Vector3 rightdir = Quaternion.AngleAxis(angle / 2, axis) * Vector3.forward;
        int pcount = (int)angle / 10;
        //顶点
        Vector3[] vertexs = new Vector3[3 + pcount];
        vertexs[0] = Vector3.zero;
        int index = 1;
        vertexs[index] = leftdir * radius;
        index++;
        for (int i = 0; i < pcount; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(10 * i, axis) * leftdir;
            vertexs[index] = dir * radius;
            index++;
        }
        vertexs[index] = rightdir * radius;
        //三角面
        int[] triangles = new int[3 * (1 + pcount)];
        for (int i = 0; i < 1 + pcount; i++)
        {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 1;
            triangles[3 * i + 2] = i + 2;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertexs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }



}
