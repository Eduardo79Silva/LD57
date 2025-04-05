#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CollapseBlock.generated.h"

UCLASS()
class YOURPROJECTNAME_API ACollapseBlock : public AActor
{
    GENERATED_BODY()
	
public:	
    ACollapseBlock();

    UPROPERTY(VisibleAnywhere)
    UStaticMeshComponent* BlockMesh;

    virtual void BeginPlay() override;
};
