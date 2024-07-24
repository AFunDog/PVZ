shader_type canvas_item;

uniform bool hit = false;
uniform bool frozen = false;

void fragment() {
	float current_a = texture(TEXTURE,UV).a;
	vec4 hit_color = mix(texture(TEXTURE,UV),vec4(vec3(1),current_a),float(hit) * 0.4);
	vec4 frozen_color = mix(texture(TEXTURE,UV),vec4(0,0,1,current_a),float(frozen) * 0.2);
	vec4 target_color = mix(hit_color,frozen_color,0.5);
	COLOR = target_color;
}

