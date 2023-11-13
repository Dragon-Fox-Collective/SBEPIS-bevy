use bevy::prelude::*;
use bevy_xpbd_3d::prelude::*;

use crate::with_only;

#[derive(Component)]
pub struct Hand;

pub fn move_hand_to_position(
	mut body: Query<(&mut Rotation, &GlobalTransform), with_only!(PlayerBody)>,
	mut hand: Query<(&mut Position, &mut Rotation), with_only!(Hand)>,
)
{
	let (_, body_transform) = body.single_mut();
	let (mut position, mut rotation) = hand.single_mut();
	position.0 = body_transform.transform_point(Vec3::Y * 2.);
	rotation.0 = body_transform.compute_transform().rotation;
}

// struct Foo(with_only![Hand]);
// const FOO: Foo = Foo(());

// struct Bar(with_only![PlayerBody]);
// const BAR: Bar = Bar(());