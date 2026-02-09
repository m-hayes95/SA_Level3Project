using System.Collections;

// Does not attatch to a game object in the scene so does not derrive from mono
// Abstract so needs to be defined by another class
public abstract class State 
{
    public virtual void Enter() { }
    public virtual IEnumerator Task() { 
        // Defaut to yeild break if this method is not overitten
        yield break;
    }
    public virtual void Exit() { }
}
