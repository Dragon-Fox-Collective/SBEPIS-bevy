use bevy::prelude::*;
use bevy_xpbd_3d::prelude::*;

use super::PlayerBody;

#[derive(Component)]
pub struct Hand;

#[derive(Component)]
pub struct HandJoint;

pub fn move_hand_to_position(
	mut body: Query<&GlobalTransform, With<PlayerBody>>,
	mut hand: Query<(&mut Position, &mut Rotation), With<Hand>>,
)
{
	let body_transform = body.single_mut();
	let (mut position, mut rotation) = hand.single_mut();
	position.0 = body_transform.transform_point(Vec3::Y * 2.);
	rotation.0 = body_transform.compute_transform().rotation;
}