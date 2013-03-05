
function Drop () {
	collider.isTrigger = false;
	if (!rigidbody)
		gameObject.AddComponent(Rigidbody);
}