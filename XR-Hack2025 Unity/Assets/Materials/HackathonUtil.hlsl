float4 POSITIONS[8];

void GetHighlight_float(float3 worldPos, out float distance)
{
    float minDist = 999;
    for(int i=0; i < 8; i++){
        if(POSITIONS[i].w == 0) continue;
        float3 delta = worldPos - POSITIONS[i].xyz;
        float dist = length(delta);
        minDist = min(minDist, dist);
    }
    distance = minDist;
}