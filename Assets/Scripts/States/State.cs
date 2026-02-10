using System.Collections;
using UnityEngine;

// Does not attatch to a game object in the scene so does not derrive from mono
// Abstract so needs to be defined by another class
public abstract class State 
{
    protected Enemy enemy;
    // Constructer to pass in data about enemy context
    public State(Enemy enemy) {  this.enemy = enemy; }
    
    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual IEnumerator Task() { 
        // Defaut to yeild break if this method is not overitten
        yield break;
    }
    public virtual void Exit() { }

    public virtual void OnTriggerEnter(Collider collider) { }
    public virtual void OnTriggerExit(Collider collider) { }
    public virtual void OnTriggerStay(Collider collider ) { }
}
